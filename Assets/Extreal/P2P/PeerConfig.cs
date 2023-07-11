using SocketIOClient;
using Unity.WebRTC;

namespace Extreal.P2P.Dev
{
    public class PeerConfig
    {
        public string Url { get; private set; }
        public SocketIOOptions SocketIOOptions { get; private set; }
        public RTCConfiguration PcConfig { get; private set; }

        public PeerConfig(string url, SocketIOOptions socketIOOptions = null, RTCConfiguration pcConfig = new RTCConfiguration())
        {
            Url = url;
            SocketIOOptions = socketIOOptions ?? new SocketIOOptions();
            PcConfig = pcConfig;
        }
    }
}
