using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.MultiplayCommon;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

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
        private readonly AssetHelper assetHelper;
        private readonly string avatarAssetName;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<string, GameObject> loadedAvatars = new Dictionary<string, GameObject>();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayRoom));

        public MultiplayRoom(NgoClient ngoClient, NgoConfig ngoConfig, AssetHelper assetHelper, string avatarAssetName)
        {
            this.ngoClient = ngoClient;
            this.ngoConfig = ngoConfig;
            this.assetHelper = assetHelper;
            this.avatarAssetName = avatarAssetName;

            this.ngoClient.OnConnected
                .Subscribe(_ =>
                    ngoClient.RegisterMessageHandler(MessageName.PlayerSpawned.ToString(), PlayerSpawnedMessageHandler))
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
            foreach (var avatar in loadedAvatars)
            {
                Addressables.Release(avatar.Value);
            }
        }

        public async UniTask JoinAsync()
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
            messagePayload.ReadValueSafe(out SpawnedMessage spawnedMessage);
            var spawnedObject = SpawnedObjects[spawnedMessage.NetworkObjectId];
            if (spawnedObject.IsOwner)
            {
                HandleOwnerAsync(spawnedMessage, spawnedObject).Forget();
            }
            else
            {
                SetAvatarAsync(spawnedObject, spawnedMessage.AvatarAssetName).Forget();
            }
        }

        private async UniTask HandleOwnerAsync(SpawnedMessage spawnedMessage, NetworkObject spawnedObject)
        {
            Controller(spawnedObject).AvatarAssetName.Value = spawnedMessage.AvatarAssetName;
            SetAvatarForExistingSpawnedObjects(ownerId: spawnedMessage.NetworkObjectId);
            await SetAvatarAsync(spawnedObject, spawnedMessage.AvatarAssetName);
            isPlayerSpawned.Value = true;
        }

        private static Dictionary<ulong, NetworkObject> SpawnedObjects
            => NetworkManager.Singleton.SpawnManager.SpawnedObjects;

        private static NetworkThirdPersonController Controller(NetworkObject networkObject)
            => networkObject.GetComponent<NetworkThirdPersonController>();

        private void SetAvatarForExistingSpawnedObjects(ulong ownerId)
        {
            foreach (var existingObject in SpawnedObjects.Values)
            {
                if (ownerId != existingObject.NetworkObjectId)
                {
                    string avatarName = Controller(existingObject).AvatarAssetName.Value;
                    SetAvatarAsync(existingObject, avatarName, restore: true).Forget();
                }
            }
        }

        private async UniTask SetAvatarAsync(NetworkObject networkObject, string avatarAssetName, bool restore = false)
        {
            var prefab = await LoadAvatarAsync(avatarAssetName);
            var avatarObject = Object.Instantiate(prefab, networkObject.transform);
            Controller(networkObject).SetAvatar(avatarObject.GetComponent<AvatarProvider>().Avatar, restore);
        }

        public async UniTask<GameObject> LoadAvatarAsync(string avatarAssetName)
        {
            if (!loadedAvatars.TryGetValue(avatarAssetName, out var avatar))
            {
                avatar = await assetHelper.LoadAssetAsync<GameObject>(avatarAssetName);
                loadedAvatars.Add(avatarAssetName, avatar);
            }
            return avatar;
        }
    }
}
