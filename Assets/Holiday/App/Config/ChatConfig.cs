using System.Diagnostics.CodeAnalysis;
using Extreal.Integration.Chat.OME;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(ChatConfig),
        fileName = nameof(ChatConfig))]
    public class ChatConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private bool initialMute = true;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private float initialInVolume = 1f;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private float initialOutVolume = 1f;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private float audioLevelCheckIntervalSeconds = 1f;

        public VoiceChatConfig VoiceChatConfig
            => new VoiceChatConfig(initialMute, initialInVolume, initialOutVolume, audioLevelCheckIntervalSeconds);
    }
}
