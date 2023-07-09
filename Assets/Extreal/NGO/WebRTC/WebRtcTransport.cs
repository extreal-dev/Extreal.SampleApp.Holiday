﻿using System;
using Extreal.Core.Logging;
using Unity.Netcode;
using UnityEngine;

namespace Extreal.NGO.WebRTC.Dev
{
    public class WebRtcTransport : NetworkTransport
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(WebRtcTransport));

        private WebRtcClient webRtcClient;

        public void SetWebRtcClient(WebRtcClient webRtcClient) => this.webRtcClient = webRtcClient;

        public override void Send(ulong clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
            => webRtcClient.Send(clientId, payload);

        public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
        {
            var evt = webRtcClient is not null ? webRtcClient.PollEvent() : WebRtcEvent.Nothing;
            clientId = evt.ClientId;
            payload = evt.Payload != null ? new ArraySegment<byte>(evt.Payload) : new ArraySegment<byte>();
            receiveTime = Time.realtimeSinceStartup;
            return evt.Type;
        }

        public override bool StartClient() => StartAs(WebRtcRole.Client);

        public override bool StartServer() => StartAs(WebRtcRole.Host);

        private bool StartAs(WebRtcRole role)
        {
            webRtcClient.Connect(role);
            return true;
        }

        public override void DisconnectRemoteClient(ulong clientId)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(DisconnectRemoteClient)}: clientId={clientId}");
            }
        }

        public override void DisconnectLocalClient()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(DisconnectLocalClient)}");
            }
            webRtcClient.Disconnect();
        }

        public override ulong GetCurrentRtt(ulong clientId) => 100;

        public override void Shutdown()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(Shutdown)}");
            }
            webRtcClient.Shutdown();
        }

        public override void Initialize(NetworkManager networkManager = null)
        {
        }

        public override ulong ServerClientId => 0;
    }
}
