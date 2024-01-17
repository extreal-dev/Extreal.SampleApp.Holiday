using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Extreal.Integration.SFU.OME;
using UnityEngine;

namespace Extreal.SampleApp.Holiday
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(SfuConfig),
        fileName = nameof(SfuConfig))]
    public class SfuConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string serverUrl = "ws://localhost:3000";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private List<IceServer> iceServers;

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

        public OmeConfig OmeConfig
            => new OmeConfig(
                serverUrl,
                iceServers.Count > 0
                    ? iceServers.Select(iceServer => new IceServerConfig(iceServer.Urls, iceServer.Username, iceServer.Credential)).ToList()
                    : default);
    }
}
