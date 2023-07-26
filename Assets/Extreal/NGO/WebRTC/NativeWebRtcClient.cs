﻿#if !UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.P2P.Dev;
using Unity.Netcode;
using Unity.WebRTC;

namespace Extreal.NGO.WebRTC.Dev
{
    public class NativeWebRtcClient : WebRtcClient
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(NativeWebRtcClient));
        private static readonly string Label = "multiplay";

        private readonly Dictionary<string, RTCDataChannel> dcDict;
        private readonly IdMapper idMapper;
        private readonly HashSet<ulong> disconnectedRemoteClients;
        private readonly NativePeerClient peerClient;
        private CancellationTokenSource cancellation;

        public NativeWebRtcClient(NativePeerClient peerClient)
        {
            dcDict = new Dictionary<string, RTCDataChannel>();
            idMapper = new IdMapper();
            disconnectedRemoteClients = new HashSet<ulong>();
            this.peerClient = peerClient;
            cancellation = new CancellationTokenSource();

            peerClient.AddPcCreateHook(CreatePc);
            peerClient.AddPcCloseHook(ClosePc);
        }

        private void CreatePc(string id, bool isOffer, RTCPeerConnection pc)
        {
            if (dcDict.ContainsKey(id))
            {
                return;
            }

            // In NGO, The client connects only to the host.
            // The host connects to all clients.
            if (peerClient.Role == PeerRole.Client && id != peerClient.HostId)
            {
                return;
            }

            if (isOffer)
            {
                var dc = pc.CreateDataChannel(Label);
                HandleDc(id, dc);
            }
            else
            {
                pc.OnDataChannel += (dc) => HandleDc(id, dc);
            }
        }

        private void HandleDc(string id, RTCDataChannel dc)
        {
            if (dc.Label != Label)
            {
                return;
            }

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"New DataChannel: id={id} label={dc.Label}");
            }

            dcDict.Add(id, dc);
            idMapper.Add(id);
            var clientId = idMapper.Get(id);

            // Host only
            if (peerClient.Role == PeerRole.Host)
            {
                dc.OnOpen = () =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"{nameof(dc.OnOpen)}: clientId={clientId}");
                    }
                    FireOnConnected(clientId);
                };
            }

            // Both Host and Client
            dc.OnMessage = message => FireOnDataReceived(clientId, Encoding.ASCII.GetString(message));
            dc.OnClose = () =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"{nameof(dc.OnClose)}: clientId={clientId}");
                }

                if (peerClient.Role == PeerRole.Host && disconnectedRemoteClients.Remove(clientId))
                {
                    return;
                }
                FireOnDisconnected(clientId);
            };
        }

        private void ClosePc(string id)
        {
            if (!dcDict.TryGetValue(id, out var pc))
            {
                return;
            }
            pc.Close();
            dcDict.Remove(id);
            idMapper.Remove(id);
        }

        protected override async UniTask DoConnectAsync()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(Connect)}: role={peerClient.Role}");
            }

            if (peerClient.Role == PeerRole.Client)
            {
                var hostId = peerClient.HostId;
                await UniTask.WaitUntil(() => idMapper.Has(hostId), cancellationToken: cancellation.Token);
                var clientId = GetHostId(nameof(Connect), hostId);
                if (!IsHostIdNotFound(clientId))
                {
                    FireOnConnected(clientId);
                }
            }
        }

        private const ulong HostIdNotFound = 0;
        private static bool IsHostIdNotFound(ulong hostId) => hostId == HostIdNotFound;

        private ulong GetHostId(string caller, string hostId)
        {
            ulong result;
            if (hostId is not null)
            {
                result = idMapper.Has(hostId) ? idMapper.Get(hostId) : HostIdNotFound;
            }
            else
            {
                result = HostIdNotFound;
            }
            if (Logger.IsDebug() && caller != nameof(Send))
            {
                Logger.LogDebug($"{nameof(GetHostId)}: caller={caller} hostId={hostId}");
            }
            return result;
        }

        protected override void DoSend(ulong clientId, string payload)
        {
            var fixedClientId =
                clientId != NetworkManager.ServerClientId
                    ? clientId
                    : GetHostId(nameof(Send), peerClient.HostId);
            if (!idMapper.TryGet(fixedClientId, out var id))
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"DoSend: id not found. clientId={clientId}");
                }
                return;
            }
            dcDict[id].Send(payload);
        }

        protected override void DoClear()
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = new CancellationTokenSource();
            disconnectedRemoteClients.Clear();
            dcDict.Keys.ToList().ForEach(ClosePc);
            dcDict.Clear();
            idMapper.Clear();
        }

        public override void DisconnectRemoteClient(ulong clientId)
            => disconnectedRemoteClients.Add(clientId);

        protected override void ReleaseManagedResources() => cancellation?.Dispose();
    }
}
#endif
