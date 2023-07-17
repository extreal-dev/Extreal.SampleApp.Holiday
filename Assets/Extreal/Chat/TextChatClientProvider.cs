using System.Diagnostics.CodeAnalysis;
using Extreal.P2P.Dev;

namespace Extreal.Chat.Dev
{
    public class TextChatClientProvider
    {
        [SuppressMessage("Style", "CC0038")]
        public static TextChatClient Provide(PeerClient peerClient)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return new NativeTextChatClient(peerClient as NativePeerClient);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            return new WebGLTextChatClient();
#endif
        }
    }
}
