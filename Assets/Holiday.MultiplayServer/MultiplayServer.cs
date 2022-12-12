using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.MultiplayCommon;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Extreal.SampleApp.Holiday.MultiplayServer
{
    public class MultiplayServer : IDisposable
    {
        private readonly MultiplayServerConfig multiplayServerConfig;
        private readonly NgoServer ngoServer;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayServer));

        public MultiplayServer(MultiplayServerConfig multiplayServerConfig, NgoServer ngoServer)
        {
            this.multiplayServerConfig = multiplayServerConfig;
            this.ngoServer = ngoServer;
        }

        public void Initialize()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"MaxCapacity: {multiplayServerConfig.MaxCapacity}");
            }

            ngoServer.SetConnectionApprovalCallback((_, response) =>
            {
                response.Approved = ngoServer.ConnectedClients.Count < multiplayServerConfig.MaxCapacity;
            });

            ngoServer.OnServerStarted
                .Subscribe(_ => ngoServer.RegisterMessageHandler(MessageName.PlayerSpawn.ToString(), PlayerSpawnMessageHandler))
                .AddTo(disposables);

            /* TODO Allow MessageHandler to be unregistered even if the server is down
            ngoServer.OnServerStopping
                .Subscribe(_ => ngoServer.UnregisterMessageHandler(MessageName.PlayerSpawn.ToString()))
                .AddTo(disposables);
            */
        }

        public void Dispose()
        {
            ngoServer.StopServerAsync().Forget();
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public UniTask StartAsync()
            => ngoServer.StartServerAsync();

        private void SendPlayerSpawned(ulong clientId)
        {
            var messageStream = new FastBufferWriter(FixedString64Bytes.UTF8MaxLengthInBytes, Allocator.Temp);
            ngoServer.SendMessageToClients(new List<ulong> { clientId }, MessageName.PlayerSpawned.ToString(), messageStream);
        }

        private async void PlayerSpawnMessageHandler(ulong clientId, FastBufferReader messageStream)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{MessageName.PlayerSpawn}: {clientId}");
            }
            messageStream.ReadValueSafe(out string avatarAssetName);
            var result = Addressables.LoadAssetAsync<GameObject>(avatarAssetName);
            var playerPrefab = await result.Task;
            ngoServer.SpawnAsPlayerObject(clientId, playerPrefab);

            SendPlayerSpawned(clientId);
        }
    }
}
