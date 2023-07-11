using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using SocketIOClient;
using UniRx;
using Unity.WebRTC;

namespace Extreal.P2P.Dev
{
    public class NativePeerClient : PeerClient
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(NativePeerClient));

        private SocketIO socket;
        private readonly Dictionary<string, RTCPeerConnection> pcDict;
        private readonly List<Action<string, bool, RTCPeerConnection>> pcCreateHooks;
        private readonly List<Action<string>> pcCloseHooks;

        private readonly ClientState clientState;

        public NativePeerClient(PeerConfig peerConfig) : base(peerConfig)
        {
            pcDict = new Dictionary<string, RTCPeerConnection>();
            pcCreateHooks = new List<Action<string, bool, RTCPeerConnection>>();
            pcCloseHooks = new List<Action<string>>();
            clientState = new ClientState();
            Disposables.Add(clientState);

            clientState.OnStarted.Subscribe(_ => FireOnStarted()).AddTo(Disposables);
        }

        private class ClientState : DisposableBase
        {
            private readonly CompositeDisposable disposables = new CompositeDisposable();

            public IObservable<Unit> OnStarted => onStarted.AddTo(disposables);
            private readonly Subject<Unit> onStarted = new Subject<Unit>();

            private readonly BoolReactiveProperty iceCandidateGathering = new BoolReactiveProperty(false);
            private readonly BoolReactiveProperty offerAnswerProcess = new BoolReactiveProperty(false);

            public ClientState()
            {
                iceCandidateGathering.AddTo(disposables);
                offerAnswerProcess.AddTo(disposables);
                Observable.CombineLatest(iceCandidateGathering, offerAnswerProcess)
                    .Where(readies => readies.All(ready => ready))
                    .Subscribe(_ => onStarted.OnNext(Unit.Default))
                    .AddTo(disposables);
            }

            public void FinishIceCandidateGathering() => iceCandidateGathering.Value =true;
            public void FinishOfferAnswerProcess() => offerAnswerProcess.Value = true;

            public void Clear()
            {
                iceCandidateGathering.Value = false;
                offerAnswerProcess.Value = false;
            }

            protected override void ReleaseManagedResources() => disposables.Dispose();
        }

        protected override void DoReleaseManagedResources() => socket?.Dispose();

        public void AddPcCreateHook(Action<string, bool, RTCPeerConnection> hook)
            => pcCreateHooks.Add(hook);

        public void AddPcCloseHook(Action<string> hook)
            => pcCloseHooks.Add(hook);

        private async UniTask CreateSocketIfNotAlreadyAsync()
        {
            if (socket is not null)
            {
                return;
            }

            socket = new SocketIO(PeerConfig.Url, PeerConfig.SocketIOOptions);
            socket.On("message", ReceiveMessage);
            socket.On("user disconnected", ReceiveUserDisconnected);

            await socket.ConnectAsync();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Socket created: id={socket.Id}");
            }
        }

        private async void ReceiveMessage(SocketIOResponse response)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Receive message: {response}");
            }

            var message = response.GetValue<Message>();

            switch (message.Type)
            {
                case "join":
                {
                    ReceiveJoin(message.From);
                    break;
                }
                case "call me":
                {
                    await SendOfferAsync(message.Me);
                    break;
                }
                case "offer":
                {
                    await ReceiveOfferAsync(message.From, message.ToSd());
                    break;
                }
                case "answer":
                {
                    await ReceiveAnswerAsync(message.From, message.ToSd());
                    break;
                }
                case "done":
                {
                    ReceiveDone(message.From);
                    break;
                }
                case "candidate":
                {
#pragma warning disable CC0022
                    ReceiveCandidate(message.From, new RTCIceCandidate(message.Ice.ToInit()));
#pragma warning restore CC0022
                    break;
                }
                case "bye":
                {
                    ReceiveBye(message.From);
                    break;
                }
                default:
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"Unknown message received!!! type={message.Type}");
                    }
                    break;
                }
            }
        }

        private void ReceiveUserDisconnected(SocketIOResponse response)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Receive user disconnected: {response}");
            }

            var userDisconnected = response.GetValue<UserDisconnected>();
            ClosePc(userDisconnected.Id);
        }

        protected override async UniTask DoStartHostAsync(string name)
        {
            await CreateSocketIfNotAlreadyAsync();

            StartHostResponse startHostResponse = null;
            await socket.EmitAsync("create host", response =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(response.ToString());
                }
                startHostResponse = response.GetValue<StartHostResponse>();
            }, name);
            await UniTask.WaitUntil(() => startHostResponse != null);

            if (startHostResponse.Status == 409)
            {
                throw new HostNameAlreadyExistsException(startHostResponse.Message);
            }

            FireOnStarted();
        }

        public override async UniTask<List<Host>> ListHostsAsync()
        {
            await CreateSocketIfNotAlreadyAsync();

            ListHostsResponse listHostsResponse = null;
            await socket.EmitAsync("list hosts", response =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(response.ToString());
                }
                listHostsResponse = response.GetValue<ListHostsResponse>();
            });
            await UniTask.WaitUntil(() => listHostsResponse != null);

            return listHostsResponse.Hosts.Select(host => new Host(host.Id, host.Name)).ToList();
        }

        protected override async UniTask DoStartClientAsync()
        {
            await CreateSocketIfNotAlreadyAsync();
            await SendMessageAsync(HostId, new Message { Type = "join" });
        }

        private async UniTask SendOfferAsync(string to)
        {
            if (pcDict.ContainsKey(to))
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Send offer: Not sent as it already exists. to={to}");
                }
                return;
            }

            CreatePc(to, true);

            await HandlePcAsync(
                nameof(SendOfferAsync),
                to,
                async pc =>
                {
                    var offerAsyncOp = pc.CreateOffer();
                    await offerAsyncOp;
                    var sd = offerAsyncOp.Desc;
                    await pc.SetLocalDescription(ref sd);
                    await SendSdpAsync(to, pc.LocalDescription);
                });
        }

        protected override async UniTask DoStopAsync()
        {
            clientState.Clear();

            foreach (var id in pcDict.Keys.ToList())
            {
                await SendMessageAsync(id, new Message { Type = "bye" });
                ClosePc(id);
            }
            pcDict.Clear();

            socket?.Dispose();
            socket = null;
        }

        private void CreatePc(string id, bool isOffer)
        {
            var pcConfig = PeerConfig.PcConfig;
            var pc = new RTCPeerConnection(ref pcConfig);

            pc.OnIceCandidate = async e =>
            {
                if (string.IsNullOrEmpty(e.Candidate))
                {
                    return;
                }
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Receive ice candidate: state={e.Candidate} id={id}");
                }
                await SendIceAsync(id, e);
            };

            pc.OnIceConnectionChange = _ =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Receive ice connection change: state={pc.IceConnectionState} id={id}");
                }
                switch (pc.IceConnectionState)
                {
                    case RTCIceConnectionState.New:
                    case RTCIceConnectionState.Checking:
                    case RTCIceConnectionState.Disconnected:
                    {
                        // do nothing
                        break;
                    }
                    case RTCIceConnectionState.Connected:
                    case RTCIceConnectionState.Completed:
                    {
                        if (Role == PeerRole.Client)
                        {
                            clientState.FinishIceCandidateGathering();
                        }
                        break;
                    }
                    case RTCIceConnectionState.Failed:
                    case RTCIceConnectionState.Closed:
                    {
                        ClosePc(id);
                        break;
                    }
                }
            };

            pcCreateHooks.ForEach(hook => hook.Invoke(id, isOffer, pc));
            pcDict.Add(id, pc);
        }

        private void ClosePc(string from)
            => HandlePc(
                nameof(ClosePc),
                from,
                pc =>
                {
                    pcCloseHooks.ForEach(hook => hook.Invoke(from));
                    pc.Close();
                    pcDict.Remove(from);
                });

        private UniTask SendSdpAsync(string to, RTCSessionDescription sd)
            => SendMessageAsync(to, new Message
            {
                Type = sd.type.ToString().ToLower(),
                Sdp = sd.sdp,
            });

        private UniTask SendIceAsync(string to, RTCIceCandidate ice)
            => HandlePcAsync(
                nameof(SendIceAsync),
                to,
                _ => SendMessageAsync(to,
                    new Message
                    {
                        Type = "candidate",
                        Ice = new Ice
                        {
                            Candidate = ice.Candidate,
                            SdpMid = ice.SdpMid,
                            SdpMLineIndex = ice.SdpMLineIndex
                        }
                    }));

        [SuppressMessage("Design", "CC0021")]
        public async UniTask SendMessageAsync(string to, Message message)
        {
            message.To = to;
            await socket.EmitAsync("message", message);
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Send message: {message}");
            }
        }

        private void ReceiveJoin(string from)
        {
            SendOfferAsync(from).Forget();
            foreach (var to in pcDict.Keys)
            {
                if (from == to)
                {
                    continue;
                }
                SendMessageAsync(to, new Message { Type = "call me", Me = from }).Forget();
            }
        }

        private async UniTask ReceiveOfferAsync(string from, RTCSessionDescription sd)
        {
            CreatePc(from, false);

            await HandlePcAsync(
                nameof(ReceiveOfferAsync),
                from,
                async pc =>
                {
                    await pc.SetRemoteDescription(ref sd);
                    await SendAnswerAsync(from);
                });
        }

        private UniTask SendAnswerAsync(string from)
            => HandlePcAsync(
                nameof(SendAnswerAsync),
                from,
                async pc =>
                {
                    var answerAsyncOp = pc.CreateAnswer();
                    await answerAsyncOp;
                    var sd = answerAsyncOp.Desc;
                    await pc.SetLocalDescription(ref sd);
                    await SendSdpAsync(from, pc.LocalDescription);
                });

        private UniTask ReceiveAnswerAsync(string from, RTCSessionDescription sd)
            => HandlePcAsync(
                nameof(ReceiveAnswerAsync),
                from,
                async pc =>
                {
                    await pc.SetRemoteDescription(ref sd);
                    await SendMessageAsync(from, new Message { Type = "done" });
                });

        private void ReceiveDone(string from)
        {
            if (Role == PeerRole.Client)
            {
                clientState.FinishOfferAnswerProcess();
            }
        }

        private void ReceiveCandidate(string from, RTCIceCandidate candidate)
            => HandlePc(
                nameof(ReceiveCandidate),
                from,
                pc => pc.AddIceCandidate(candidate));

        private void ReceiveBye(string from) => ClosePc(from);

        private void HandlePc(
            string methodName,
            string id,
            Action<RTCPeerConnection> handle)
        {
            if (!pcDict.ContainsKey(id))
            {
                return;
            }

            var pc = pcDict[id];
            try
            {
                handle.Invoke(pc);
            }
            catch (Exception e)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Error has occurred at {methodName}", e);
                }
            }
        }

        private async UniTask HandlePcAsync(
            string methodName,
            string id,
            Func<RTCPeerConnection, UniTask> handle)
        {
            if (!pcDict.ContainsKey(id))
            {
                return;
            }

            var pc = pcDict[id];
            try
            {
                await handle.Invoke(pc);
            }
            catch (Exception e)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Error has occurred at {methodName}", e);
                }
            }
        }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class Message
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

#pragma warning disable CS8632

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("me")]
        public string? Me { get; set; }

        [JsonPropertyName("sdp")]
        public string? Sdp { get; set; }

        [JsonPropertyName("ice")]
        public Ice? Ice { get; set; }

#pragma warning restore CS8632

        private static readonly Dictionary<string, RTCSdpType> TypeMapping
            = new Dictionary<string, RTCSdpType>
            {
                {"offer", RTCSdpType.Offer},
                {"answer", RTCSdpType.Answer},
            };

        public RTCSessionDescription ToSd()
            => new RTCSessionDescription
            {
                type = TypeMapping[Type],
                sdp = Sdp,
            };

        public override string ToString()
            => $"{nameof(Type)}: {Type}, {nameof(From)}: {From}, {nameof(To)}: {To}, "
               + $"{nameof(Me)}: {Me}, {nameof(Sdp)}: {Sdp}, {nameof(Ice)}: {Ice}";
    }

    public class Ice
    {
        [JsonPropertyName("candidate")]
        public string Candidate { get; set; }

        [JsonPropertyName("sdpMid")]
        public string SdpMid { get; set; }

        [JsonPropertyName("sdpMLineIndex")]
        public int? SdpMLineIndex { get; set; }

        public RTCIceCandidateInit ToInit()
            => new RTCIceCandidateInit
            {
                candidate = Candidate,
                sdpMid = SdpMid,
                sdpMLineIndex = SdpMLineIndex,
            };

        public override string ToString()
            => $"{nameof(Candidate)}: {Candidate}, {nameof(SdpMid)}: {SdpMid}, "
               + $"{nameof(SdpMLineIndex)}: {SdpMLineIndex}";
    }

    [SuppressMessage("Usage", "CC0047")]
    public class UserDisconnected
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class StartHostResponse
    {
        [JsonPropertyName("status")]
        public ushort Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class ListHostsResponse
    {
        [JsonPropertyName("status")]
        public ushort Status { get; set; }

        [JsonPropertyName("hosts")]
        public List<HostResponse> Hosts { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class HostResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
