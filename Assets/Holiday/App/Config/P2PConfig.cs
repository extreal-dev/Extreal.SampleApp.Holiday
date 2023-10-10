using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int socketConnectionTimeoutSeconds = 5;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private List<string> iceServerUrls = new List<string>();
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int p2pTimeoutSeconds = 15;

        public PeerConfig PeerConfig
            => new PeerConfig(
                signalingUrl,
                new SocketIOOptions
                {
                    ConnectionTimeout = TimeSpan.FromSeconds(socketConnectionTimeoutSeconds),
                    Reconnection = false,
                },
                iceServerUrls,
                p2pTimeoutSeconds
                );
    }
}
