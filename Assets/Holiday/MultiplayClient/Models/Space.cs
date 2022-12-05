using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.Common;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Models
{
    public class Space : IInitializable, IDisposable
    {
        public IObservable<Unit> OnConnected => ngoClient.OnConnected;

        public IObservable<Unit> OnDisconnected => onDisconnected;
        private readonly Subject<Unit> onDisconnected = new Subject<Unit>();

        public IObservable<bool> IsPlayerSpawned => isPlayerSpawned;
        private readonly BoolReactiveProperty isPlayerSpawned = new BoolReactiveProperty(false);

        private readonly NgoClient ngoClient;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(Space));

        public Space(NgoClient ngoClient)
            => this.ngoClient = ngoClient;

        public void Initialize()
        {
            ngoClient.OnConnected
                .Subscribe(_ => ngoClient.RegisterMessageHandler(MessageName.PlayerSpawned.ToString(), PlayerSpawnedMessageHandler))
                .AddTo(disposables);

            ngoClient.OnDisconnecting
                .Subscribe(_ =>
                {
                    isPlayerSpawned.Value = false;
                    ngoClient.UnregisterMessageHandler(MessageName.PlayerSpawned.ToString());
                })
                .AddTo(disposables);
        }

        public void Dispose()
        {
            isPlayerSpawned.Dispose();
            onDisconnected.Dispose();
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public async UniTask JoinAsync()
        {
            var ngoConfig = new NgoConfig();
            await ngoClient.ConnectAsync(ngoConfig);
        }

        public async UniTask LeaveAsync()
        {
            await ngoClient.DisconnectAsync();
            onDisconnected.OnNext(Unit.Default);
        }

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
