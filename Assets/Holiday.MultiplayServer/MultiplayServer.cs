using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.MultiplayCommon;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayServer
{
    public class MultiplayServer : DisposableBase
    {
        private readonly NgoServer ngoServer;
        private readonly GameObject playerPrefab;

        private bool isDisposed;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayServer));

        public MultiplayServer(NgoServer ngoServer, GameObject playerPrefab)
        {
            this.ngoServer = ngoServer;
            this.playerPrefab = playerPrefab;
        }

        public void Initialize()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"MaxCapacity: {MultiplayServerArgumentHandler.MaxCapacity}");
            }

            ngoServer.SetConnectionApprovalCallback((_, response) =>
                response.Approved = ngoServer.ConnectedClients.Count < MultiplayServerArgumentHandler.MaxCapacity);

            ngoServer.OnServerStarted
                .Subscribe(_ =>
                    ngoServer.RegisterMessageHandler(MessageName.PlayerSpawn.ToString(), PlayerSpawnMessageHandler))
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
        {
            isDisposed = true;
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }

        public async UniTask StartAsync()
        {
            DestroyInLifetimeSecondsAsync().Forget();
            OutputMemoryStatisticsAsync().Forget();
            await ngoServer.StartServerAsync();
        }

        private void SendPlayerSpawned(ulong objectId)
        {
            var messageStream = new FastBufferWriter(FixedString64Bytes.UTF8MaxLengthInBytes, Allocator.Temp);
            messageStream.WriteValueSafe(objectId);
            ngoServer.SendMessageToAllClients(MessageName.PlayerSpawned.ToString(), messageStream);
        }

        private void PlayerSpawnMessageHandler(ulong clientId, FastBufferReader messageStream)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{MessageName.PlayerSpawn}: {clientId}");
            }

            messageStream.ReadValueSafe(out string avatarAssetName);
            var spawnedPlayer = ngoServer.SpawnAsPlayerObject(clientId, playerPrefab);
            spawnedPlayer.GetComponent<NetworkThirdPersonController>().AvatarPath = avatarAssetName;

            var spawnedObjectId = spawnedPlayer.GetComponent<NetworkObject>().NetworkObjectId;
            SendPlayerSpawned(spawnedObjectId);
        }

        private async UniTaskVoid OutputMemoryStatisticsAsync()
        {
            var path = MultiplayServerArgumentHandler.MemoryUtilizationDumpFile;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (System.IO.File.Exists(path))
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"There already exists a file at {path}");
                }
                return;
            }

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Creates a file {path} and writes data into it");
            }

            var file = System.IO.File.Create(path);
            var writer = new System.IO.StreamWriter(file, System.Text.Encoding.UTF8);
            writer.WriteLine("Date Time TotalReservedMemory TotalAllocatedMemory TotalUnusedReservedMemory");

            while (!isDisposed)
            {
                var currentTime = DateTime.Now;
                var totalReservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
                var totalAllocatedMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
                var totalUnusedReservedMemory = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();
                writer.WriteLine($"{currentTime} {totalReservedMemory} {totalAllocatedMemory} {totalUnusedReservedMemory}");

                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cts.Token);
            }

            file.Close();
        }

        private static async UniTaskVoid DestroyInLifetimeSecondsAsync()
        {
            if (MultiplayServerArgumentHandler.Lifetime == 0f)
            {
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(MultiplayServerArgumentHandler.Lifetime));

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }
    }
}
