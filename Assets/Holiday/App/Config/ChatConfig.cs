using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Integration.Chat.OME;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(ChatConfig),
        fileName = nameof(ChatConfig))]
    public class ChatConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string serverUrl = "ws://localhost:3000";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private List<IceServer> iceServers;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private bool initialMute = true;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private float initialInVolume = 1f;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private float initialOutVolume = 1f;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private float audioLevelCheckIntervalSeconds = 1f;

        [Serializable]
        public class IceServer
        {
            [SerializeField] private List<string> urls;
            [SerializeField] private string username;
            [SerializeField] private string credential;

            public List<string> Urls => urls;
            public string Username => username;
            public string Credential => credential;
        }

        public VoiceChatConfig VoiceChatConfig
            => new VoiceChatConfig(
                serverUrl,
                iceServers.Count > 0
                    ? iceServers.Select(iceServer => new IceServerConfig(iceServer.Urls, iceServer.Username, iceServer.Credential)).ToList()
                    : default,
                initialMute,
                initialInVolume,
                initialOutVolume,
                audioLevelCheckIntervalSeconds);
    }
}
