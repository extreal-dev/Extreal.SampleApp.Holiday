using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.AssetWorkflow.Addressables;
using Extreal.Integration.Messaging;
using Extreal.Integration.Multiplay.Messaging;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.Controls.Common.Multiplay;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl.Client
{
    public class MultiplayRoom : DisposableBase
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

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayRoom));

        public MultiplayRoom(MultiplayClient multiplayClient, QueuingMessagingClient messagingClient, GameObject playerPrefab, AssetHelper assetHelper, AppState appState)
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
            var groupName = $"Multiplay#{appState.Space.SpaceName}";
            if (!groups.Select(group => group.Name).Contains(groupName))
            {
                var groupConfig = new GroupConfig(groupName, assetHelper.MultiplayConfig.MaxCapacity);
                try
                {
                    await messagingClient.CreateGroupAsync(groupConfig);
                }
                catch (GroupNameAlreadyExistsException e)
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug("Group name already existed", e);
                    }
                }
            }

            var messagingJoiningConfig = new MessagingJoiningConfig(groupName);
            var joiningConfig = new MultiplayJoiningConfig(messagingJoiningConfig);
            await multiplayClient.JoinAsync(joiningConfig);
        }

        public async UniTaskVoid LeaveAsync()
            => await multiplayClient.LeaveAsync();

        private async UniTaskVoid SetOwnerAvatarAsync(string avatarAssetName)
        {
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
            var remoteAvatarName = tuple.messageJson;
            HandleReceivedAvatarName(userIdentityRemote, remoteAvatarName);
        }

        private void HandleReceivedAvatarName(string userIdentityRemote, string avatarAssetName)
        {
            var spawnedObject = ConnectedUsers[userIdentityRemote].NetworkObjects[0];
            const bool isOwner = false;
            SetAvatarAsync(spawnedObject, avatarAssetName, isOwner).Forget();
        }

        private static NetworkThirdPersonController Controller(GameObject gameObject)
            => gameObject.GetComponent<NetworkThirdPersonController>();

        private async UniTask SetAvatarAsync(GameObject gameObject, string avatarAssetName, bool isOwner)
        {
            var assetDisposable = await LoadAvatarAsync(avatarAssetName);

            if (gameObject.GetComponentInChildren<AvatarProvider>() == null)
            {
                var avatarObject = Object.Instantiate(assetDisposable.Result, gameObject.transform);
                Controller(gameObject).Initialize(avatarObject.GetComponent<AvatarProvider>().Avatar, isOwner, AppUtils.IsTouchDevice());
            }
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
