using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly QueuingMessagingClient messagingClient;
        private readonly AssetHelper assetHelper;
        private readonly AppState appState;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<string, AssetDisposable<GameObject>> loadedAvatars
            = new Dictionary<string, AssetDisposable<GameObject>>();

        private IReadOnlyDictionary<string, NetworkClient> ConnectedUsers => multiplayClient.JoinedUsers;

        private RedisThirdPersonController myAvatar;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MassivelyMultiplayClient));

        public MassivelyMultiplayClient(MultiplayClient multiplayClient, QueuingMessagingClient messagingClient, GameObject playerPrefab, AssetHelper assetHelper, AppState appState)
        {
            this.multiplayClient = multiplayClient;
            this.messagingClient = messagingClient;
            this.assetHelper = assetHelper;
            this.appState = appState;

            this.multiplayClient.OnJoined
                .Subscribe(_ => multiplayClient.SpawnObject(playerPrefab, message: appState.Avatar.AssetName))
                .AddTo(disposables);

            this.multiplayClient.OnObjectSpawned
                .Subscribe(HandleObjectSpawned)
                .AddTo(disposables);

            this.multiplayClient.OnUserJoined
                .Subscribe(userId => multiplayClient.SendMessage(appState.Avatar.AssetName, userId))
                .AddTo(disposables);

            this.multiplayClient.OnMessageReceived
                .Subscribe(HandleReceivedMessage)
                .AddTo(disposables);

            this.multiplayClient.OnLeaving
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
            var groups = await messagingClient.ListGroupsAsync();
            if (!groups.Select(group => group.Name).Contains(appState.GroupName))
            {
                var groupConfig = new GroupConfig(appState.GroupName, assetHelper.NgoHostConfig.MaxCapacity);
                await messagingClient.CreateGroupAsync(groupConfig);
            }

            var messagingJoiningConfig = new MessagingJoiningConfig(appState.GroupName);
            var joiningConfig = new MultiplayJoiningConfig(messagingJoiningConfig);
            await multiplayClient.JoinAsync(joiningConfig);
        }

        public async UniTaskVoid LeaveAsync()
        {
            if (appState.IsHost)
            {
                await messagingClient.DeleteGroupAsync(appState.GroupName);
            }
            await multiplayClient.LeaveAsync();
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
            myAvatar = Controller(multiplayClient.LocalClient.NetworkObjects[0]);
            myAvatar.AvatarAssetName.Value = (NetworkString)avatarAssetName;
            const bool isOwner = true;
            await SetAvatarAsync(multiplayClient.LocalClient.NetworkObjects[0], avatarAssetName, isOwner);
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
            var spawnedObject = ConnectedUsers[userIdentityRemote].NetworkObjects[0];
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
