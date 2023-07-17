using Extreal.Core.Logging;

namespace Extreal.P2P.Dev
{
    public class WebGLPeerConfig : PeerConfig
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(WebGLPeerConfig));

        public WebGLPeerConfig(PeerConfig peerConfig)
            : base(peerConfig.SignalingUrl, peerConfig.SocketOptions, peerConfig.IceServerUrls)
        {
        }

        public bool IsDebug => Logger.IsDebug();
    }
}
