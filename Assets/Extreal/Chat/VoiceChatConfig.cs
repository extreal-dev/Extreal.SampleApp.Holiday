namespace Extreal.Chat.Dev
{
    public class VoiceChatConfig
    {
        public bool InitialMute { get; private set; }

        public VoiceChatConfig(bool initialMute = true) => InitialMute = initialMute;
    }
}
