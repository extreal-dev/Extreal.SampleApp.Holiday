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
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int hostMaxCapacity = 10;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int clientTimeoutSeconds = 5;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int clientMaxRetryCount = 3;

        public HostConfig HostConfig => new HostConfig(hostMaxCapacity);
        public ClientConfig ClientConfig => new ClientConfig(clientTimeoutSeconds, clientMaxRetryCount);
    }

    public class HostConfig
    {
        public NgoConfig NgoConfig { get; private set; }
        public int MaxCapacity { get; private set; }
        public HostConfig(int maxCapacity)
        {
            NgoConfig = new NgoConfig();
            MaxCapacity = maxCapacity;
        }
    }

    public class ClientConfig
    {
        public NgoConfig NgoConfig { get; private set; }
        public IRetryStrategy RetryStrategy { get; private set; }
        public ClientConfig(int timeoutSeconds, int maxRetryCount)
        {
            NgoConfig = new NgoConfig(timeout: TimeSpan.FromSeconds(timeoutSeconds));
            RetryStrategy = new CountingRetryStrategy(maxRetryCount);
        }
    }
}
