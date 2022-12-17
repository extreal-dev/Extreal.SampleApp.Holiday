using System;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.MultiplayCommon;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
                response.Approved = ngoServer.ConnectedClients.Count < multiplayServerConfig.MaxCapacity);

            ngoServer.OnServerStarted
                .Subscribe(_ =>
                    ngoServer.RegisterMessageHandler(MessageName.PlayerSpawn.ToString(), PlayerSpawnMessageHandlerAsync))
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public async UniTask StartAsync()
        {
            var unityTransport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && IPAddress.TryParse(args[1], out var ipAddress))
            {
                unityTransport.ConnectionData.Address = ipAddress.ToString();
            }
            if (args.Length > 2 && ushort.TryParse(args[1], out var port))
            {
                unityTransport.ConnectionData.Port = port;
            }

            await ngoServer.StartServerAsync();
        }

        private void SendPlayerSpawned(ulong clientId)
        {
            var messageStream = new FastBufferWriter(FixedString64Bytes.UTF8MaxLengthInBytes, Allocator.Temp);
            ngoServer.SendMessageToClients(new List<ulong> { clientId }, MessageName.PlayerSpawned.ToString(),
                messageStream);
        }

        private async void PlayerSpawnMessageHandlerAsync(ulong clientId, FastBufferReader messageStream)
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
