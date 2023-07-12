using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.AssetWorkflow.Addressables;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.P2P;
using Extreal.SampleApp.Holiday.Controls.Common.Multiplay;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extreal.SampleApp.Holiday.Controls.MultiplyControl.Client
{
    public class MultiplayClient : DisposableBase
    {
        public IObservable<bool> IsPlayerSpawned => isPlayerSpawned;
        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty isPlayerSpawned = new BoolReactiveProperty(false);

        private readonly NgoClient ngoClient;
        private readonly AssetHelper assetHelper;
        private readonly AppState appState;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<string, AssetDisposable<GameObject>> loadedAvatars
            = new Dictionary<string, AssetDisposable<GameObject>>();

        private static Dictionary<ulong, NetworkObject> SpawnedObjects
            => NetworkManager.Singleton.SpawnManager.SpawnedObjects;

        private NetworkThirdPersonController myAvatar;
        private bool isConnected;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayClient));

        public MultiplayClient(NgoClient ngoClient, AssetHelper assetHelper, AppState appState)
        {
            this.ngoClient = ngoClient;
            this.assetHelper = assetHelper;
            this.appState = appState;

            // FIXME:
            // ホストの場合はクライアントが接続してくる度に OnConnected の通知が送られてきます。
            // そのため自分自身がサーバーと接続したときだけ処理が走るように isConnected で制御しています。
            this.ngoClient.OnConnected
                .Where(_ => !isConnected)
                .Subscribe(_ =>
                {
                    isConnected = true;
                    ngoClient.RegisterMessageHandler(MessageName.PlayerSpawned.ToString(), PlayerSpawnedMessageHandler);
                    ngoClient.RegisterMessageHandler(MessageName.ReceivedFromEveryone.ToString(), ReceivedFromEveryoneMessageHandler);
                    SendPlayerSpawn(appState.Avatar.AssetName);
                })
                .AddTo(disposables);

            this.ngoClient.OnDisconnecting
                .Subscribe(_ =>
                {
                    isPlayerSpawned.Value = false;
                    ngoClient.UnregisterMessageHandler(MessageName.PlayerSpawned.ToString());
                })
                .AddTo(disposables);

            this.ngoClient.OnUnexpectedDisconnected
                .Where(_ => appState.IsClient)
                .Subscribe(_ => isConnected = false)
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
        {
            cts.Cancel();
            cts.Dispose();
            isPlayerSpawned.Dispose();
            disposables.Dispose();
        }

        public async UniTaskVoid JoinAsync()
            => await ngoClient.ConnectAsync(assetHelper.NgoClientConfig.NgoConfig, cts.Token);

        public async UniTaskVoid LeaveAsync() => await ngoClient.DisconnectAsync();

        public void ResetPosition()
            => myAvatar.ResetPosition();

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

        public void SendToEveryone(Message message)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(
                    "Send message spread to server" + Environment.NewLine
                    + $" message ID: {message.MessageId}" + Environment.NewLine
                    + $" content: {message.Content}");
            }

            var messageStream = new FastBufferWriter(FixedString512Bytes.UTF8MaxLengthInBytes, Allocator.Temp);
            messageStream.WriteValueSafe(message);
            ngoClient.SendMessage(MessageName.SendToEveryone.ToString(), messageStream);
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

        private void ReceivedFromEveryoneMessageHandler(ulong senderClientId, FastBufferReader messagePayload)
        {
            messagePayload.ReadValueSafe(out Message message);

            if (Logger.IsDebug())
            {
                Logger.LogDebug(
                    "Received message spread from server" + Environment.NewLine
                    + $" message ID: {message.MessageId}" + Environment.NewLine
                    + $" parameter: {message.Content}");
            }

            appState.ReceivedMessage(message);
        }

        private async UniTaskVoid HandleOwnerAsync(SpawnedMessage spawnedMessage, NetworkObject spawnedObject)
        {
            myAvatar = Controller(spawnedObject);
            myAvatar.AvatarAssetName.Value = spawnedMessage.AvatarAssetName;

            SetAvatarForExistingSpawnedObjects(ownerId: spawnedMessage.NetworkObjectId);
            await SetAvatarAsync(spawnedObject, spawnedMessage.AvatarAssetName);
            isPlayerSpawned.Value = true;
        }

        private static NetworkThirdPersonController Controller(NetworkObject networkObject)
            => networkObject.GetComponent<NetworkThirdPersonController>();

        private void SetAvatarForExistingSpawnedObjects(ulong ownerId)
        {
            foreach (var existingObject in SpawnedObjects.Values.ToArray())
            {
                if (ownerId != existingObject.NetworkObjectId)
                {
                    string avatarName = Controller(existingObject).AvatarAssetName.Value;
                    SetAvatarAsync(existingObject, avatarName).Forget();
                }
            }
        }

        private async UniTask SetAvatarAsync(NetworkObject networkObject, string avatarAssetName)
        {
            var assetDisposable = await LoadAvatarAsync(avatarAssetName);
            var avatarObject = Object.Instantiate(assetDisposable.Result, networkObject.transform);
            Controller(networkObject)
                .Initialize(avatarObject.GetComponent<AvatarProvider>().Avatar, AppUtils.IsTouchDevice());
        }

        public async UniTask<AssetDisposable<GameObject>> LoadAvatarAsync(string avatarAssetName)
        {
            if (!loadedAvatars.TryGetValue(avatarAssetName, out var assetDisposable))
            {
                assetDisposable = await assetHelper.LoadAssetAsync<GameObject>(avatarAssetName);
                if (loadedAvatars.TryAdd(avatarAssetName, assetDisposable))
                {
                    disposables.Add(assetDisposable);
                }
                else
                {
                    assetDisposable.Dispose();
                    assetDisposable = loadedAvatars[avatarAssetName];
                }
            }
            return assetDisposable;
        }
    }
}
