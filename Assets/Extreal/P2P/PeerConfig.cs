using Unity.WebRTC;

namespace Extreal.P2P.Dev
{
    public class PeerConfig
    {
        public string Url { get; private set; }
        public RTCConfiguration PcConfig { get; private set; }

        public PeerConfig(string url, RTCConfiguration pcConfig = new RTCConfiguration())
        {
            Url = url;
            PcConfig = pcConfig;
        }
    }
}
