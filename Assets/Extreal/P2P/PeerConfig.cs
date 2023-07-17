using System.Collections.Generic;
using SocketIOClient;

namespace Extreal.P2P.Dev
{
    public class PeerConfig
    {
        public string SignalingUrl { get; private set; }
        public SocketIOOptions SocketOptions { get; private set; }
        public List<string> IceServerUrls { get; private set; }

        public PeerConfig(string url, SocketIOOptions socketOptions = null, List<string> iceServerUrls = null)
        {
            SignalingUrl = url;
            SocketOptions = socketOptions ?? new SocketIOOptions();
            IceServerUrls = iceServerUrls ?? new List<string>();
        }
    }
}
