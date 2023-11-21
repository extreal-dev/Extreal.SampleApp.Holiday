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
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using LiveKit;

namespace Extreal.SampleApp.Holiday.Controls.MultiplyControl.Client
{
    public class MultiplayClient : DisposableBase
    {
        public IObservable<bool> IsPlayerSpawned => isPlayerSpawned;
        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty isPlayerSpawned = new BoolReactiveProperty(false);

        private readonly LiveKitMultiplayClient liveKitMultiplayClient;
        private readonly AssetHelper assetHelper;
        private readonly List<string> avatarNames;
        private readonly AppState appState;
        private readonly string userIdentity;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<string, AssetDisposable<GameObject>> loadedAvatars
            = new Dictionary<string, AssetDisposable<GameObject>>();

        private readonly Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

        private NetworkThirdPersonController myAvatar;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayClient));

        public MultiplayClient(LiveKitMultiplayClient liveKitMultiplayClient, AssetHelper assetHelper, AppState appState)
        {
            this.liveKitMultiplayClient = liveKitMultiplayClient;
            this.assetHelper = assetHelper;
            this.appState = appState;
            spawnedObjects = liveKitMultiplayClient.ConnectedClients.ToDictionary(c => c.Participant.Identity, c => c.PlayerObject);
            userIdentity = appState.PlayerName + "-" + Guid.NewGuid();

            this.liveKitMultiplayClient.OnConnected
                .Subscribe(_ =>
                {
                    liveKitMultiplayClient.Spawn(liveKitMultiplayClient.ConnectedClients.Select(client => client.PlayerObject).ToList());
                    SetOwnerAvatarAsync(appState.Avatar.AssetName);
                    SendPlayerAvatarName(appState.Avatar.AssetName);
                })
                .AddTo(disposables);

            this.liveKitMultiplayClient.OnMessageReceived
                .Subscribe(HandleReceivedMessage)
                .AddTo(disposables);

            this.liveKitMultiplayClient.OnDisconnecting
                .Subscribe(_ =>
                {
                    isPlayerSpawned.Value = false;
                    SendPlayerDespawn();
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

        public async UniTaskVoid JoinAsync()
            => await liveKitMultiplayClient.ConnectAsync("url", "accessToken");

        public async UniTaskVoid LeaveAsync() => await liveKitMultiplayClient.DisconnectAsync();

        public void ResetPosition() => myAvatar.ResetPosition();

        private void SendPlayerAvatarName(string avatarAssetName)
        {
            var messageJson = JsonUtility.ToJson(avatarAssetName);
            liveKitMultiplayClient.SendMessage(MessageName.PlayerSpawn.ToString(), messageJson, DataPacketKind.RELIABLE);
        }

        private void SendPlayerDespawn()
        {
            var message = new LiveKitMultiplayMessage(LiveKidMultiplayMessageCommand.Delete);
            var messageJson = message.ToJson();
            liveKitMultiplayClient.SendMessage(MessageName.PlayerSpawn.ToString(), messageJson, DataPacketKind.RELIABLE);
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
            liveKitMultiplayClient.SendMessage(MessageName.SendToEveryone.ToString(), messageJson, DataPacketKind.RELIABLE);
        }

        private async UniTaskVoid SetOwnerAvatarAsync(string avatarAssetName) => await SetAvatarAsync(spawnedObjects[userIdentity], avatarAssetName);

        private void HandleReceivedMessage((Participant participant, string messageJson) tuple)
        {
            var messageSpaceTransition = JsonUtility.FromJson<Message>(tuple.messageJson);
            if (messageSpaceTransition.MessageId == MessageId.SpaceTransition)
            {
                HandleReceivedMessageSpaceTransition(messageSpaceTransition);
            }

            var avatarNames = assetHelper.AvatarConfig.Avatars.Select(avatar => avatar.Name).ToList();
            var messageAvatarName = JsonUtility.FromJson<string>(tuple.messageJson);
            if (avatarNames.Contains(messageAvatarName))
            {
                HandleReceivedMessageAvatarName(tuple.participant, messageAvatarName);
            }
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

        private void HandleReceivedMessageAvatarName(Participant participant, string avatarAssetName)
        {
            var spawnedObject = spawnedObjects[participant.Identity];
            SetAvatarAsync(spawnedObject, avatarAssetName).Forget();
        }

        private async UniTaskVoid HandleOwnerAsync(SpawnedMessage spawnedMessage, GameObject spawnedObject)
        {
            myAvatar = Controller(spawnedObject);
            myAvatar.AvatarAssetName.Value = spawnedMessage.AvatarAssetName;

            SetAvatarForExistingSpawnedObjects(ownerId: spawnedMessage.NetworkObjectId);
            await SetAvatarAsync(spawnedObject, spawnedMessage.AvatarAssetName);
            isPlayerSpawned.Value = true;
        }

        private static NetworkThirdPersonController Controller(GameObject networkObject)
            => networkObject.GetComponent<NetworkThirdPersonController>();

        private void SetAvatarForExistingSpawnedObjects(ulong ownerId)
        {
            foreach (var existingObject in spawnedObjects.Values.ToArray())
            {
                // if (ownerId != existingObject.NetworkObjectId)
                // {
                string avatarName = Controller(existingObject).AvatarAssetName.Value;
                SetAvatarAsync(existingObject, avatarName).Forget();
                // }
            }
        }

        private async UniTask SetAvatarAsync(GameObject networkObject, string avatarAssetName)
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
                    // Not covered by testing due to defensive implementation
                    assetDisposable.Dispose();
                    assetDisposable = loadedAvatars[avatarAssetName];
                }
            }
            return assetDisposable;
        }
    }
}
