using System.Diagnostics.CodeAnalysis;

namespace Extreal.P2P.Dev
{
    public static class PeerClientProvider
    {
        [SuppressMessage("Style", "CC0038")]
        public static PeerClient Provide(PeerConfig peerConfig)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return new NativePeerClient(peerConfig);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            return null;
#endif
        }
    }
}
