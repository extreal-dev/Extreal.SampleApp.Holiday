using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Retry;
using Extreal.Integration.Multiplay.NGO;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(MultiplayConfig),
        fileName = nameof(MultiplayConfig))]
    public class MultiplayConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string address = "127.0.0.1";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private ushort port = 7777;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int timeoutSeconds = 5;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int maxRetryCount = 8;

        public NgoConfig NgoConfig => new NgoConfig(address, port, timeout: TimeSpan.FromSeconds(timeoutSeconds));
        public IRetryStrategy RetryStrategy => new CountingRetryStrategy(maxRetryCount);
    }
}
