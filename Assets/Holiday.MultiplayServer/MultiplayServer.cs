﻿using System;
using System.Collections.Generic;
using System.Threading;
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
        private readonly NgoServer ngoServer;

        private bool isDisposed;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayServer));

        public MultiplayServer(NgoServer ngoServer)
            => this.ngoServer = ngoServer;

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
                    ngoServer.RegisterMessageHandler(MessageName.PlayerSpawn.ToString(), PlayerSpawnMessageHandlerAsync))
                .AddTo(disposables);
        }

        public void Dispose()
        {
            isDisposed = true;
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public async UniTask StartAsync()
        {
            DestroyInLifetimeSecondsAsync().Forget();
            OutputMemoryStatisticsAsync().Forget();
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
