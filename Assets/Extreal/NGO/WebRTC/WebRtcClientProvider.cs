using System.Diagnostics.CodeAnalysis;
using Extreal.P2P.Dev;

namespace Extreal.NGO.WebRTC.Dev
{
    public static class WebRtcClientProvider
    {
        [SuppressMessage("Style", "CC0038")]
        public static WebRtcClient Provide(PeerClient peerClient)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return new NativeWebRtcClient(peerClient as NativePeerClient);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            return null;
#endif
        }
    }
}
