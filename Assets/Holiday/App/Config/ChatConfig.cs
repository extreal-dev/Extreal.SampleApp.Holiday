using System.Diagnostics.CodeAnalysis;
using Extreal.Chat.Dev;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(ChatConfig),
        fileName = nameof(ChatConfig))]
    public class ChatConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private bool initialMute = true;

        public VoiceChatConfig VoiceChatConfig => new VoiceChatConfig(initialMute);
    }
}
