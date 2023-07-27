using System.Diagnostics.CodeAnalysis;
using Extreal.Integration.P2P.WebRTC;

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
            return new WebGLWebRtcClient();
#endif
        }
    }
}
