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
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string hostAddress = "127.0.0.1";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private ushort hostPort = 7777;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int hostMaxCapacity = 10;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string clientAddress = "127.0.0.1";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private ushort clientPort = 7777;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int clientTimeoutSeconds = 5;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int clientMaxRetryCount = 3;

        public HostConfig HostConfig => new HostConfig(hostAddress, hostPort, hostMaxCapacity);
        public ClientConfig ClientConfig => new ClientConfig(clientAddress, clientPort, clientTimeoutSeconds, clientMaxRetryCount);
    }

    public class HostConfig
    {
        public NgoConfig NgoConfig { get; private set; }
        public int MaxCapacity { get; private set; }
        public HostConfig(string address, ushort port, int maxCapacity)
        {
            NgoConfig = new NgoConfig(address, port);
            MaxCapacity = maxCapacity;
        }
    }

    public class ClientConfig
    {
        public NgoConfig NgoConfig { get; private set; }
        public IRetryStrategy RetryStrategy { get; private set; }
        public ClientConfig(string address, ushort port, int timeoutSeconds, int maxRetryCount)
        {
            NgoConfig = new NgoConfig(address, port, timeout: TimeSpan.FromSeconds(timeoutSeconds));
            RetryStrategy = new CountingRetryStrategy(maxRetryCount);
        }
    }
}
