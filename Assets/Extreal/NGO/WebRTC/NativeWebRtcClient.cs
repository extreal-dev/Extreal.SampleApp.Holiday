using System;
using System.Collections.Generic;
using System.Linq;
using Extreal.Core.Logging;
using Extreal.P2P.Dev;
using Unity.Netcode;
using Unity.WebRTC;

namespace Extreal.NGO.WebRTC.Dev
{
    public class NativeWebRtcClient : WebRtcClient
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(NativeWebRtcClient));

        private readonly Dictionary<string, List<RTCDataChannel>> dcDict;
        private readonly IdMapper idMapper;
        private readonly Queue<WebRtcEvent> events;
        private readonly PeerClient peerClient;

        public NativeWebRtcClient(NativePeerClient peerClient)
        {
            dcDict = new Dictionary<string, List<RTCDataChannel>>();
            idMapper = new IdMapper();
            events = new Queue<WebRtcEvent>();
            this.peerClient = peerClient;
            peerClient.AddPcCreateHook(CreatePc);
            peerClient.AddPcCloseHook(ClosePc);
        }

        private void CreatePc(string id, bool isOffer, RTCPeerConnection pc)
        {
            if (peerClient.Role == PeerRole.Client && id != peerClient.HostId)
            {
                return;
            }

            if (isOffer)
            {
                var dc = pc.CreateDataChannel("multiplay");
                HandleDc(id, true, dc);
            }
            else
            {
                pc.OnDataChannel = (dc) => HandleDc(id, false, dc);
            }
        }

        private void HandleDc(string id, bool isOffer, RTCDataChannel dc)
        {
            if (!dcDict.ContainsKey(id))
            {
                dcDict.Add(id, new List<RTCDataChannel>());
                idMapper.Add(id);
            }

            dcDict[id].Add(dc);
            var clientId = idMapper.Get(id);

            if (isOffer) // Host only
            {
                dc.OnOpen = () =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"{nameof(dc.OnOpen)}: clientId={clientId}");
                    }
                    events.Enqueue(new WebRtcEvent(NetworkEvent.Connect, clientId));
                };
            }

            // Both Host and Client
            dc.OnMessage = message =>
            {
                if (dc.ReadyState != RTCDataChannelState.Open)
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"{nameof(dc.OnMessage)}: DataChannel is not open. clientId={clientId}");
                    }
                    return;
                }
                events.Enqueue(new WebRtcEvent(NetworkEvent.Data, clientId, ToByte(message)));
            };
            dc.OnClose = () =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"{nameof(dc.OnClose)}: clientId={clientId}");
                }
                events.Enqueue(new WebRtcEvent(NetworkEvent.Disconnect, clientId));
            };
        }

        private static byte[] ToByte(byte[] message)
        {
            var strBuf = System.Text.Encoding.ASCII.GetString(message);
            var str2Array = strBuf.Split('-');
            var byteBuf = new byte[str2Array.Length];
            for(var i = 0; i < str2Array.Length; i++){
                byteBuf[i] = Convert.ToByte(str2Array[i], 16);
            }
            return byteBuf;
        }

        private void ClosePc(string id)
        {
            if (!dcDict.ContainsKey(id))
            {
                return;
            }
            dcDict[id].ForEach(dc => dc.Close());
            dcDict.Remove(id);
            idMapper.Remove(id);
        }

        public override void Connect(WebRtcRole role)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(Connect)}: role={role}");
            }
            Role = role;
            if (Role == WebRtcRole.Client)
            {
                var hostId = GetHostId();
                if (hostId != HostIdNotFound)
                {
                    events.Enqueue(new WebRtcEvent(NetworkEvent.Connect, hostId));
                }
            }
        }

        private static readonly ulong HostIdNotFound = 0;

        private ulong GetHostId()
        {
            var hostId = peerClient.HostId;
            if (hostId is null)
            {
                return HostIdNotFound;
            }
            return idMapper.Has(hostId) ? idMapper.Get(hostId) : HostIdNotFound;
        }

        public override void Send(ulong clientId, ArraySegment<byte> payload)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                clientId = idMapper.Get(peerClient.HostId);
            }

            if (!idMapper.Has(clientId))
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"{nameof(Send)}: clientId not found. clientId={clientId}");
                }
                return;
            }

            var id = idMapper.Get(clientId);
            dcDict[id].ForEach((dc) =>
            {
                if (dc.ReadyState != RTCDataChannelState.Open)
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"{nameof(Send)}: DataChannel is not open. clientId={clientId}");
                    }
                    return;
                }
                dc.Send(ToStr(payload));
            });
        }

        private static string ToStr(ArraySegment<byte> payload)
        {
            if (0 < payload.Offset || payload.Count < payload.Array.Length)
            {
                var buf = new byte[payload.Count];
                Buffer.BlockCopy(payload.Array!, payload.Offset, buf, 0, payload.Count);
                return BitConverter.ToString(buf);
            }

            return BitConverter.ToString(payload.Array);
        }

        public override WebRtcEvent PollEvent() => events.Count > 0 ? events.Dequeue() : WebRtcEvent.Nothing;

        public override void Disconnect()
        {
            if (Role == WebRtcRole.Client)
            {
                var hostId = GetHostId();
                if (hostId != HostIdNotFound)
                {
                    events.Enqueue(new WebRtcEvent(NetworkEvent.Disconnect, hostId));
                }
            }
        }

        public override void Shutdown()
        {
            dcDict.Keys.ToList().ForEach(ClosePc);
            dcDict.Clear();
            idMapper.Clear();
            events.Clear();
        }
    }
}
