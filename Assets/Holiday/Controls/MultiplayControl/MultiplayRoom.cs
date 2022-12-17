using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.MultiplayCommon;
using UniRx;
using Unity.Collections;
using Unity.Netcode;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayRoom : IDisposable
    {
        public IObservable<Unit> OnConnectionApprovalRejected => ngoClient.OnConnectionApprovalRejected;
        public IObservable<Unit> OnUnexpectedDisconnected => ngoClient.OnUnexpectedDisconnected;

        public IObservable<Unit> OnConnectFailed => onConnectFailed;
        private readonly Subject<Unit> onConnectFailed = new Subject<Unit>();

        public IObservable<bool> IsPlayerSpawned => isPlayerSpawned;
        private readonly BoolReactiveProperty isPlayerSpawned = new BoolReactiveProperty(false);

        private readonly NgoClient ngoClient;
        private readonly NgoConfig ngoConfig;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MultiplayRoom));

        public MultiplayRoom(NgoClient ngoClient, NgoConfig ngoConfig)
        {
            this.ngoClient = ngoClient;
            this.ngoConfig = ngoConfig;

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

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            onConnectFailed.Dispose();
            isPlayerSpawned.Dispose();
            disposables.Dispose();
            GC.SuppressFinalize(this);
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

        public void SendPlayerSpawn(string avatarAssetName)
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
            => isPlayerSpawned.Value = true;
    }
}
