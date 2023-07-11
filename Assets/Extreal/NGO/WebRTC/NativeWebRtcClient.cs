using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private readonly Dictionary<string, List<RTCDataChannel>> dcDict;
        private readonly IdMapper idMapper;
        private readonly Queue<WebRtcEvent> events;

        private readonly NativePeerClient peerClient;
        private string connectedHostId; // using Disconnect method only
        private PeerRole connectedRole; // using Disconnect method only

        private readonly HashSet<ulong> disconnectedRemoteClients;

        public NativeWebRtcClient(NativePeerClient peerClient)
        {
            dcDict = new Dictionary<string, List<RTCDataChannel>>();
            idMapper = new IdMapper();
            events = new Queue<WebRtcEvent>();
            disconnectedRemoteClients = new HashSet<ulong>();

            this.peerClient = peerClient;
            connectedHostId = null;
            connectedRole = PeerRole.None;

            peerClient.AddPcCreateHook(CreatePc);
            peerClient.AddPcCloseHook(ClosePc);
        }

        private void CreatePc(string id, bool isOffer, RTCPeerConnection pc)
        {
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

            if (!dcDict.ContainsKey(id))
            {
                dcDict.Add(id, new List<RTCDataChannel>());
                idMapper.Add(id);
            }

            dcDict[id].Add(dc);
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
                    events.Enqueue(new WebRtcEvent(NetworkEvent.Connect, clientId));
                };
            }

            // Both Host and Client
            dc.OnMessage = message =>
            {
                events.Enqueue(new WebRtcEvent(NetworkEvent.Data, clientId, ToByte(message)));
            };
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
                events.Enqueue(new WebRtcEvent(NetworkEvent.Disconnect, clientId));
            };
        }

        private static byte[] ToByte(byte[] message)
        {
            var strBuf = Encoding.ASCII.GetString(message);
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

        public override void Connect()
        {
            connectedHostId = peerClient.HostId;
            connectedRole = peerClient.Role;
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(Connect)}: role={connectedRole}");
            }

            if (peerClient.Role == PeerRole.Client)
            {
                FireNetworkEvent(peerClient.HostId, NetworkEvent.Connect);
            }
        }

        private void FireNetworkEvent(string hostIdOrNull, NetworkEvent networkEvent)
        {
            var hostId = GetHostId(hostIdOrNull);
            if (hostId != HostIdNotFound)
            {
                events.Enqueue(new WebRtcEvent(networkEvent, hostId));
            }
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireNetworkEvent)}: NetworkEvent={networkEvent} hostId={hostId}");
            }
        }

        private static readonly ulong HostIdNotFound = 0;

        private ulong GetHostId(string hostId)
        {
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

        public override WebRtcEvent PollEvent()
            => events.Count > 0 ? events.Dequeue() : WebRtcEvent.Nothing;

        public override void Disconnect()
        {
            if (connectedRole == PeerRole.Client)
            {
                FireNetworkEvent(connectedHostId, NetworkEvent.Disconnect);
            }
        }

        public override void Clear()
        {
            disconnectedRemoteClients.Clear();
            connectedHostId = null;
            connectedRole = PeerRole.None;
            dcDict.Keys.ToList().ForEach(ClosePc);
            dcDict.Clear();
            idMapper.Clear();
            events.Clear();
        }

        public override void DisconnectRemoteClient(ulong clientId)
            => disconnectedRemoteClients.Add(clientId);
    }
}
