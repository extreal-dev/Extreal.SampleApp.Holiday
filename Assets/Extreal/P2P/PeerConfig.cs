using System;
using Unity.WebRTC;

namespace Extreal.P2P.Dev
{
    public class PeerConfig
    {
        public string Url { get; private set; }
        public TimeSpan Timeout { get; private set; }
        public RTCConfiguration PcConfig { get; private set; }

        public PeerConfig(string url, TimeSpan timeout, RTCConfiguration pcConfig = new RTCConfiguration())
        {
            Url = url;
            Timeout = timeout;
            PcConfig = pcConfig;
        }
    }
}
