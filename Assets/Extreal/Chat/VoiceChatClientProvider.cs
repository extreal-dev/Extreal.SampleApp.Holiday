using System.Diagnostics.CodeAnalysis;
using Extreal.P2P.Dev;
using UnityEngine;

namespace Extreal.Chat.Dev
{
    public class VoiceChatClientProvider
    {
        [SuppressMessage("Style", "CC0038")]
        public static VoiceChatClient Provide(PeerClient peerClient, AudioSource inAudio, AudioSource outAudio)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return new NativeVoiceChatClient(peerClient as NativePeerClient, inAudio, outAudio);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            return null;
#endif
        }
    }
}
