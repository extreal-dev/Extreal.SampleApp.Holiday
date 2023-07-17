using System.Diagnostics.CodeAnalysis;
using Extreal.P2P.Dev;

namespace Extreal.Chat.Dev
{
    public class VoiceChatClientProvider
    {
        [SuppressMessage("Style", "CC0038")]
        public static VoiceChatClient Provide(
            PeerClient peerClient, VoiceChatConfig config = null)
        {
            config ??= new VoiceChatConfig();
#if !UNITY_WEBGL || UNITY_EDITOR
            return new NativeVoiceChatClient(peerClient as NativePeerClient, config);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            return new WebGLVoiceChatClient(config);
#endif
        }
    }
}
