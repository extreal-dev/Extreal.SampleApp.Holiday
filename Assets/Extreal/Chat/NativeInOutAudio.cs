using UnityEngine;

namespace Extreal.Chat.Dev
{
    public class NativeInOutAudio : MonoBehaviour
    {
        public AudioSource InAudio { get; private set; }
        public AudioSource OutAudio { get; private set; }

        public void Initialize(AudioSource inAudio, AudioSource outAudio)
        {
            InAudio = inAudio;
            OutAudio = outAudio;
        }
    }
}
