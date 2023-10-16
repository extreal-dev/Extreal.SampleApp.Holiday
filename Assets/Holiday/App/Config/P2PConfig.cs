using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Integration.P2P.WebRTC;
using SocketIOClient;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(P2PConfig),
        fileName = nameof(P2PConfig))]
    public class P2PConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string signalingUrl = "http://127.0.0.1:3010";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int signalingTimeoutSeconds = 3;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int p2PTimeoutSeconds = 15;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int vanillaIceTimeoutSeconds = 5;
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

        public PeerConfig PeerConfig
            => new PeerConfig(
                signalingUrl,
                new SocketIOOptions
                {
                    ConnectionTimeout = TimeSpan.FromSeconds(signalingTimeoutSeconds),
                    Reconnection = false,
                },
                iceServers.Count > 0
                ? iceServers.Select(iceServer => new IceServerConfig(iceServer.Urls, iceServer.Username, iceServer.Credential)).ToList()
                : new List<IceServerConfig>(),
                TimeSpan.FromSeconds(p2PTimeoutSeconds),
                TimeSpan.FromSeconds(vanillaIceTimeoutSeconds));
    }
}
