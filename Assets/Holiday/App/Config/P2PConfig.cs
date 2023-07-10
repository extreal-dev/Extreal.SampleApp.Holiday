using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.P2P.Dev;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(P2PConfig),
        fileName = nameof(P2PConfig))]
    public class P2PConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string signalingUrl = "http://127.0.0.1:3010";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int timeoutSeconds = 5;

        public PeerConfig PeerConfig => new PeerConfig(signalingUrl, TimeSpan.FromSeconds(timeoutSeconds));
    }
}
