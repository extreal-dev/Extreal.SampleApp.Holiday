using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Multiplay.NGO;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Extreal.SampleApp.Holiday.App.Extreal.NGO
{
    public class NgoHost : NgoServer
    {
        private readonly NetworkManager networkManager;

        private readonly Dictionary<Type, IConnectionSetter> connectionSetters
            = new Dictionary<Type, IConnectionSetter>
            {
                {typeof(UnityTransport), new UnityTransportConnectionSetter()},
            };

        public NgoHost(NetworkManager networkManager) : base(networkManager)
            => this.networkManager = networkManager;

        public void AddConnectionSetter(IConnectionSetter connectionSetter)
        {
            if (connectionSetter == null)
            {
                throw new ArgumentNullException(nameof(connectionSetter));
            }
            connectionSetters[connectionSetter.TargetType] = connectionSetter;
        }

        public async UniTask StartHostAsync(NgoConfig ngoConfig)
        {
            var networkTransport = networkManager.NetworkConfig.NetworkTransport;
            connectionSetters[networkTransport.GetType()].Set(networkTransport, ngoConfig);

            _ = networkManager.StartHost();

            await UniTask.WaitUntil(() => networkManager.IsListening);
        }
    }
}
