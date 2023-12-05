using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.AssetWorkflow.Addressables;
using Extreal.Integration.Multiplay.LiveKit;
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

        private readonly PubSubMultiplayClient pubSubMultiplayClient;
        private readonly AssetHelper assetHelper;
        private readonly List<string> avatarNames;
        private readonly AppState appState;
        private readonly string relayUrl = "http://localhost:3030";
        private readonly ConnectionConfig connectionConfig;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<string, AssetDisposable<GameObject>> loadedAvatars
            = new Dictionary<string, AssetDisposable<GameObject>>();

        private IReadOnlyDictionary<string, NetworkClient> ConnectedClients => pubSubMultiplayClient.ConnectedClients;

        private RedisThirdPersonController myAvatar;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MassivelyMultiplayClient));

        public MassivelyMultiplayClient(PubSubMultiplayClient pubSubMultiplayClient, AssetHelper assetHelper, AppState appState)
        {
            this.pubSubMultiplayClient = pubSubMultiplayClient;
            this.assetHelper = assetHelper;
            this.appState = appState;
            connectionConfig = new ConnectionConfig(relayUrl);
            this.pubSubMultiplayClient.OnConnected
                .Subscribe(_ =>
                {
                    pubSubMultiplayClient.SpawnPlayer(message: appState.Avatar.AssetName);
                })
                .AddTo(disposables);

            this.pubSubMultiplayClient.OnObjectSpawned
                .Subscribe(HandleObjectSpawned)
                .AddTo(disposables);

            this.pubSubMultiplayClient.OnUserConnected
                .Subscribe(userIdentity =>
                {
                    pubSubMultiplayClient.SendMessage(userIdentity, appState.Avatar.AssetName);
                })
                .AddTo(disposables);

            this.pubSubMultiplayClient.OnMessageReceived
                .Subscribe(HandleReceivedMessage)
                .AddTo(disposables);

            this.pubSubMultiplayClient.OnDisconnecting
                .Subscribe(_ =>
                {
                    isPlayerSpawned.Value = false;
                })
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
        {
            cts.Cancel();
            cts.Dispose();
            isPlayerSpawned.Dispose();
            disposables.Dispose();
        }

        public async UniTaskVoid JoinAsync() => await pubSubMultiplayClient.ConnectAsync(appState.GroupName);

        public async UniTaskVoid LeaveAsync()
        {
            if (appState.IsHost)
            {
                await pubSubMultiplayClient.DeleteRoomAsync();
            }
            pubSubMultiplayClient.Disconnect();
        }

        public void ResetPosition() => myAvatar.ResetPosition();

        private void SendPlayerAvatarName(string avatarAssetName)
        {
            var userIdentityLocal = pubSubMultiplayClient.LocalClient.UserIdentity;
            pubSubMultiplayClient.SendMessage(userIdentityLocal, message: avatarAssetName);
        }

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
            pubSubMultiplayClient.SendMessage(pubSubMultiplayClient.LocalClient.UserIdentity, messageJson);
        }

        private async UniTaskVoid SetOwnerAvatarAsync(string avatarAssetName)
        {
            myAvatar = Controller(pubSubMultiplayClient.LocalClient.PlayerObject);
            myAvatar.AvatarAssetName.Value = (NetworkString)avatarAssetName;
            const bool isOwner = true;
            await SetAvatarAsync(pubSubMultiplayClient.LocalClient.PlayerObject, avatarAssetName, isOwner);
            isPlayerSpawned.Value = true;
        }

        private void HandleObjectSpawned((string userIdentity, GameObject spawnedObject, string message) tuple)
        {
            if (tuple.userIdentity == pubSubMultiplayClient.LocalClient?.UserIdentity)
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

            if (tuple.messageJson.Contains(nameof(MessageId)))
            {
                var messageSpaceTransition = JsonUtility.FromJson<Message>(tuple.messageJson);

                if (messageSpaceTransition.MessageId == MessageId.SpaceTransition)
                {

                    HandleReceivedMessageSpaceTransition(messageSpaceTransition);
                }
                return;
            }

            var remoteAvatarName = tuple.messageJson;
            HandleReceivedMessageAvatarName(userIdentityRemote, remoteAvatarName);
        }

        private void HandleReceivedMessageSpaceTransition(Message message)
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

        private void HandleReceivedMessageAvatarName(string userIdentityRemote, string avatarAssetName)
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
