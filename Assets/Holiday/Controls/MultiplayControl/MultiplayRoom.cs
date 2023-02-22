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
using UnityEngine.AddressableAssets;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayRoom : DisposableBase
    {
        public IObservable<Unit> OnConnectionApprovalRejected => ngoClient.OnConnectionApprovalRejected;
        public IObservable<Unit> OnUnexpectedDisconnected => ngoClient.OnUnexpectedDisconnected;

        public IObservable<Unit> OnConnectFailed => onConnectFailed;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onConnectFailed = new Subject<Unit>();

        public IObservable<bool> IsPlayerSpawned => isPlayerSpawned;

        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty isPlayerSpawned = new BoolReactiveProperty(false);

        private readonly NgoClient ngoClient;
        private readonly NgoConfig ngoConfig;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayRoom));

        public MultiplayRoom(NgoClient ngoClient, NgoConfig ngoConfig)
        {
            this.ngoClient = ngoClient;
            this.ngoConfig = ngoConfig;

            this.ngoClient.OnConnected
                .Subscribe(_ =>
                {
                    ngoClient.RegisterMessageHandler(MessageName.PlayerSpawned.ToString(), PlayerSpawnedMessageHandler);
                    foreach (var spawnedObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
                    {
                        SetCharacterAvatarAsync(spawnedObject).Forget();
                    }
                })
                .AddTo(disposables);

            this.ngoClient.OnDisconnecting
                .Subscribe(_ =>
                {
                    isPlayerSpawned.Value = false;
                    ngoClient.UnregisterMessageHandler(MessageName.PlayerSpawned.ToString());
                })
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
        {
            cts.Cancel();
            cts.Dispose();
            onConnectFailed.Dispose();
            isPlayerSpawned.Dispose();
            disposables.Dispose();
        }

        public async UniTask JoinAsync(string avatarAssetName)
        {
            try
            {
                await ngoClient.ConnectAsync(ngoConfig, cts.Token);
            }
            catch (TimeoutException)
            {
                onConnectFailed.OnNext(Unit.Default);
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }

            SendPlayerSpawn(avatarAssetName);
        }

        public async UniTask LeaveAsync()
            => await ngoClient.DisconnectAsync();

        private void SendPlayerSpawn(string avatarAssetName)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"spawn: avatarAssetName: {avatarAssetName}");
            }

            var messageStream = new FastBufferWriter(FixedString64Bytes.UTF8MaxLengthInBytes, Allocator.Temp);
            messageStream.WriteValueSafe(avatarAssetName);
            ngoClient.SendMessage(MessageName.PlayerSpawn.ToString(), messageStream);
        }

        private void PlayerSpawnedMessageHandler(ulong senderClientId, FastBufferReader messagePayload)
        {
            messagePayload.ReadValueSafe(out ulong objectId);
            var spawnedObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
            if (spawnedObject.IsOwner)
            {
                isPlayerSpawned.Value = true;
            }
            SetCharacterAvatarAsync(spawnedObject).Forget();
        }

        private static async UniTask SetCharacterAvatarAsync(NetworkObject spawnedObject)
        {
            var controller = spawnedObject.GetComponent<NetworkThirdPersonController>();
            await UniTask.WaitWhile(() => string.IsNullOrEmpty(controller.AvatarPath));

            var avatarPath = controller.AvatarPath;
            var result = Addressables.LoadAssetAsync<GameObject>(avatarPath);
            var playerAvatarPrefab = await result.Task;
            var playerAvatar = UnityEngine.Object.Instantiate(playerAvatarPrefab, spawnedObject.transform);

            var avatar = playerAvatar.GetComponent<AvatarProvider>().Avatar;
            var animator = spawnedObject.GetComponent<Animator>();
            controller.SaveAnimValues();
            animator.avatar = avatar;
            controller.RestoreAnimValues();
        }
    }
}
