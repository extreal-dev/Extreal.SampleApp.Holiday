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
using LiveKit;

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
        private readonly string userIdentity;
        // private readonly string accessTokenUrl = "http://localhost:3000/";
        private readonly string relayUrl = "https://massive-b001.dev.comet.ninja";
        private readonly ConnectionConfig connectionConfig;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Dictionary<string, AssetDisposable<GameObject>> loadedAvatars
            = new Dictionary<string, AssetDisposable<GameObject>>();

        private readonly Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

        private NetworkThirdPersonController myAvatar;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MassivelyMultiplayClient));

        public MassivelyMultiplayClient(PubSubMultiplayClient pubSubMultiplayClient, AssetHelper assetHelper, AppState appState)
        {
            this.pubSubMultiplayClient = pubSubMultiplayClient;
            this.assetHelper = assetHelper;
            this.appState = appState;
            spawnedObjects = pubSubMultiplayClient.ConnectedClients.ToDictionary(dic => dic.Key, dic => dic.Value.PlayerObject);
            userIdentity = appState.PlayerName + "-" + Guid.NewGuid();
            connectionConfig = new ConnectionConfig(relayUrl);

            this.pubSubMultiplayClient.OnConnected
                .Subscribe(_ =>
                {
                    pubSubMultiplayClient.SpawnPlayer();
                    Logger.LogDebug("!!!liveKitMultiplayClient.OnConnected");
                    SetOwnerAvatarAsync(appState.Avatar.AssetName).Forget();
                    // SendPlayerAvatarName(appState.Avatar.AssetName);
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

        public async UniTaskVoid JoinAsync()
        {
            // var response = await GetAccessToken(accessTokenUrl, appState.GroupName, userIdentity);
            // var accessToken = response.AccessToken;
            // var liveKitConnectionConfig = new LiveKitConnectionConfig(connectionConfig.Url, appState.GroupName, accessToken);
            await pubSubMultiplayClient.ConnectAsync(connectionConfig);
            Logger.LogDebug($"!!!Localclient after connectAsync is: {pubSubMultiplayClient.LocalClient}");
        }

        public async UniTaskVoid LeaveAsync()
        {
            pubSubMultiplayClient.Disconnect();
            // if (appState.IsHost)
            // {
            //     await liveKitMultiplayClient.DeleteRoomAsync();
            // }
        }

        public void ResetPosition() => myAvatar.ResetPosition();

        private void SendPlayerAvatarName(string avatarAssetName)
        {
            var messageJson = JsonUtility.ToJson(avatarAssetName);
            pubSubMultiplayClient.SendMessage(messageJson, DataPacketKind.RELIABLE);
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
            pubSubMultiplayClient.SendMessage(appState.GroupName, messageJson);
        }

        private async UniTaskVoid SetOwnerAvatarAsync(string avatarAssetName)
        {
            Logger.LogDebug("!!!Before SetOwnerAvatarAsync");
            const bool isOwner = true;
            await SetAvatarAsync(pubSubMultiplayClient.LocalClient.PlayerObject, avatarAssetName, isOwner);

            isPlayerSpawned.Value = true;
            Logger.LogDebug("!!!After SetOwnerAvatarAsync");
        }

        private void HandleReceivedMessage((string userIdentity, string messageJson) tuple)
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
                HandleReceivedMessageAvatarName(tuple.userIdentity, messageAvatarName);
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

        private void HandleReceivedMessageAvatarName(string userIdentity, string avatarAssetName)
        {
            var spawnedObject = spawnedObjects[userIdentity];
            var isOwner = userIdentity.Equals(userIdentity);
            SetAvatarAsync(spawnedObject, avatarAssetName, isOwner).Forget();
        }

        private static NetworkThirdPersonController Controller(GameObject gameObject)
            => gameObject.GetComponent<NetworkThirdPersonController>();

        private async UniTask SetAvatarAsync(GameObject gameObject, string avatarAssetName, bool isOwner)
        {
            Logger.LogDebug("!!!Before SetAvatarAsync");
            Logger.LogDebug("!!!Before await LoadAvatarAsync(avatarAssetName)");
            var assetDisposable = await LoadAvatarAsync(avatarAssetName);
            Logger.LogDebug("!!!After await LoadAvatarAsync(avatarAssetName)");

            Logger.LogDebug("!!!Before Object.Instantiate");
            var avatarObject = Object.Instantiate(assetDisposable.Result, gameObject.transform);
            Logger.LogDebug("!!!After Object.Instantiate");

            Controller(gameObject).Initialize(avatarObject.GetComponent<AvatarProvider>().Avatar, isOwner, AppUtils.IsTouchDevice());
            Logger.LogDebug("!!!After SetAvatarAsync");
        }

        // async UniTask<AccessTokenResponse> GetAccessToken(string url, string roomName, string userIdentity)
        // {
        //     var fullUrl = url + "getToken?RoomName=" + Uri.EscapeDataString(roomName) + "&ParticipantName=" + Uri.EscapeDataString(userIdentity);
        //     try
        //     {
        //         using var request = UnityWebRequest.Get(fullUrl);
        //         _ = await request.SendWebRequest();

        //         var json = request.downloadHandler.text;
        //         var response = JsonUtility.FromJson<AccessTokenResponse>(json);
        //         return response;
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.LogDebug(e.Message);
        //     }

        //     return null;
        // }

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

    [Serializable]
    public class AccessTokenResponse
    {
        public string AccessToken;
    }
}
