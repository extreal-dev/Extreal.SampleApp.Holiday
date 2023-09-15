using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.Controls.Common.Multiplay;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl.Host
{
    public class MultiplayHost : DisposableBase
    {
        private readonly NgoServer ngoHost;
        private readonly GameObject playerPrefab;
        private readonly AssetHelper assetHelper;


        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayHost));

        public MultiplayHost
        (
            NgoServer ngoHost,
            GameObject playerPrefab,
            AssetHelper assetHelper
        )
        {
            this.ngoHost = ngoHost;
            this.playerPrefab = playerPrefab;
            this.assetHelper = assetHelper;

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"MaxCapacity: {assetHelper.NgoHostConfig.MaxCapacity}");
            }

            this.ngoHost.SetConnectionApprovalCallback((_, response) =>
                response.Approved = ngoHost.ConnectedClients.Count < assetHelper.NgoHostConfig.MaxCapacity);

            ngoHost.OnServerStarted
                .Subscribe(_ => ngoHost.RegisterMessageHandler(MessageName.PlayerSpawn.ToString(), PlayerSpawnMessageHandler))
                .AddTo(disposables);

            ngoHost.OnServerStopping
                .Subscribe(_ =>
                {
                    ngoHost.UnregisterMessageHandler(MessageName.PlayerSpawn.ToString());
                    ngoHost.UnregisterMessageHandler(MessageName.SendToEveryone.ToString());
                })
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
        {
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }

        public UniTask StartHostAsync() => ngoHost.StartHostAsync(assetHelper.NgoHostConfig.NgoConfig);

        public UniTask StopHostAsync() => ngoHost.StopServerAsync();

        private void PlayerSpawnMessageHandler(ulong clientId, FastBufferReader messageStream)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{MessageName.PlayerSpawn}: {clientId}");
            }

            messageStream.ReadValueSafe(out string avatarAssetName);
            var spawnedPlayer = ngoHost.SpawnAsPlayerObject(clientId, playerPrefab);
            var spawnedObjectId = spawnedPlayer.GetComponent<NetworkObject>().NetworkObjectId;
            SendPlayerSpawnedToAllClients(spawnedObjectId, avatarAssetName);
        }

        private void SendPlayerSpawnedToAllClients(ulong objectId, string avatarAssetName)
        {
            var messageStream = new FastBufferWriter(FixedString64Bytes.UTF8MaxLengthInBytes, Allocator.Temp);
            messageStream.WriteValueSafe(new SpawnedMessage(objectId, avatarAssetName));
            ngoHost.SendMessageToAllClients(MessageName.PlayerSpawned.ToString(), messageStream);
        }
    }
}
