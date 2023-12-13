using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.AssetWorkflow.Addressables;
using Extreal.Integration.Messaging.Common;
using Extreal.Integration.Multiplay.Common;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.P2P;
using Extreal.SampleApp.Holiday.Controls.Common.Multiplay;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl.Client
{
    public class MassivelyMultiplayClient : DisposableBase
    {
        public IObservable<bool> IsPlayerSpawned => isPlayerSpawned;
        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty isPlayerSpawned = new BoolReactiveProperty(false);

        private readonly MultiplayClient multiplayClient;
        private readonly GroupManager groupManager;
        private readonly AssetHelper assetHelper;
        private readonly AppState appState;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<string, AssetDisposable<GameObject>> loadedAvatars
            = new Dictionary<string, AssetDisposable<GameObject>>();

        private IReadOnlyDictionary<string, NetworkClient> ConnectedClients => multiplayClient.ConnectedClients;

        private RedisThirdPersonController myAvatar;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MassivelyMultiplayClient));

        public MassivelyMultiplayClient(MultiplayClient multiplayClient, GroupManager groupManager, AssetHelper assetHelper, AppState appState)
        {
            this.multiplayClient = multiplayClient;
            this.groupManager = groupManager;
            this.assetHelper = assetHelper;
            this.appState = appState;

            this.multiplayClient.OnConnected
                .Subscribe(_ => multiplayClient.SpawnPlayer(message: appState.Avatar.AssetName))
                .AddTo(disposables);

            this.multiplayClient.OnObjectSpawned
                .Subscribe(HandleObjectSpawned)
                .AddTo(disposables);

            this.multiplayClient.OnUserConnected
                .Subscribe(userId => multiplayClient.SendMessage(appState.Avatar.AssetName, userId))
                .AddTo(disposables);

            this.multiplayClient.OnMessageReceived
                .Subscribe(HandleReceivedMessage)
                .AddTo(disposables);

            this.multiplayClient.OnDisconnecting
                .Subscribe(_ => isPlayerSpawned.Value = false)
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
        {
            var connectionConfig = new MessagingConnectionConfig(appState.GroupName, assetHelper.NgoHostConfig.MaxCapacity);
            await multiplayClient.ConnectAsync(connectionConfig);
        }

        public async UniTaskVoid LeaveAsync()
        {
            if (appState.IsHost)
            {
                await groupManager.DeleteGroupAsync();
            }
            await multiplayClient.DisconnectAsync();
        }

        public void ResetPosition() => myAvatar.ResetPosition();

        public void SendToOthers(Message message)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(
                    "Send message spread to server" + Environment.NewLine
                    + $" message ID: {message.MessageId}" + Environment.NewLine
                    + $" content: {message.Content}");
            }
            var messageJson = JsonUtility.ToJson(message);
            multiplayClient.SendMessage(messageJson);
        }

        private async UniTaskVoid SetOwnerAvatarAsync(string avatarAssetName)
        {
            myAvatar = Controller(multiplayClient.LocalClient.PlayerObject);
            myAvatar.AvatarAssetName.Value = (NetworkString)avatarAssetName;
            const bool isOwner = true;
            await SetAvatarAsync(multiplayClient.LocalClient.PlayerObject, avatarAssetName, isOwner);
            isPlayerSpawned.Value = true;
        }

        private void HandleObjectSpawned((string userIdentity, GameObject spawnedObject, string message) tuple)
        {
            if (tuple.userIdentity == multiplayClient.LocalClient?.UserId)
            {
                SetOwnerAvatarAsync(appState.Avatar.AssetName).Forget();
            }
            else if (!string.IsNullOrEmpty(tuple.message))
            {
                const bool isOwner = false;
                SetAvatarAsync(tuple.spawnedObject, tuple.message, isOwner).Forget();
            }

        }

        private void HandleReceivedMessage((string userIdentity, string messageJson) tuple)
        {
            var userIdentityRemote = tuple.userIdentity;

            if (tuple.messageJson.Contains("messageId"))
            {
                var everyoneMessage = JsonUtility.FromJson<Message>(tuple.messageJson);
                everyoneMessage.OnAfterDeserialize();
                HandleReceivedEveryoneMessage(everyoneMessage);
                return;
            }

            var remoteAvatarName = tuple.messageJson;
            HandleReceivedAvatarName(userIdentityRemote, remoteAvatarName);
        }

        private void HandleReceivedEveryoneMessage(Message message)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(
                    "Received message spread from server" + Environment.NewLine
                    + $" message ID: {message.MessageId}" + Environment.NewLine
                    + $" parameter: {message.Content}");
            }
            appState.ReceivedMessage(message);
        }

        private void HandleReceivedAvatarName(string userIdentityRemote, string avatarAssetName)
        {
            var spawnedObject = ConnectedClients[userIdentityRemote].PlayerObject;
            const bool isOwner = false;
            SetAvatarAsync(spawnedObject, avatarAssetName, isOwner).Forget();
        }

        private static RedisThirdPersonController Controller(GameObject gameObject)
            => gameObject.GetComponent<RedisThirdPersonController>();

        private async UniTask SetAvatarAsync(GameObject gameObject, string avatarAssetName, bool isOwner)
        {
            var assetDisposable = await LoadAvatarAsync(avatarAssetName);

            var avatarObject = Object.Instantiate(assetDisposable.Result, gameObject.transform);
            Controller(gameObject).Initialize(avatarObject.GetComponent<AvatarProvider>().Avatar, isOwner, AppUtils.IsTouchDevice());
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
                    // Not covered by testing due to defensive implementation
                    assetDisposable.Dispose();
                    assetDisposable = loadedAvatars[avatarAssetName];
                }
            }
            return assetDisposable;
        }
    }
}
