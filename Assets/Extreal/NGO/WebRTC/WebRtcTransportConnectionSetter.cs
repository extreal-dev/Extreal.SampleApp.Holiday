using System;
using Extreal.Integration.Multiplay.NGO;
using Unity.Netcode;

namespace Extreal.NGO.WebRTC.Dev
{
    public class WebRtcTransportConnectionSetter : IConnectionSetter
    {
        private readonly WebRtcClient webRtcClient;

        public WebRtcTransportConnectionSetter(WebRtcClient webRtcClient) => this.webRtcClient = webRtcClient;

        public Type TargetType => typeof(WebRtcTransport);

        public void Set(NetworkTransport networkTransport, NgoConfig ngoConfig)
        {
            var webRtcTransport = networkTransport as WebRtcTransport;
            webRtcTransport.SetWebRtcClient(webRtcClient);
        }
    }
}
