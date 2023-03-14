using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Integration.Chat.Vivox;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.Controls.Common
{
    public abstract class ChatChannelBase : DisposableBase
    {
        public IObservable<bool> OnConnected => onConnected;

        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty onConnected = new BoolReactiveProperty(false);

        protected VivoxClient VivoxClient { get; }
        protected ChannelId ChannelId { get; private set; }

        private readonly string channelName;

        [SuppressMessage("Usage", "CC0022")]
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        protected ChatChannelBase(VivoxClient vivoxClient, string channelName)
        {
            VivoxClient = vivoxClient;
            this.channelName = channelName;

            VivoxClient.OnUserConnected
                .Where(participant => participant.IsSelf)
                .Subscribe(_ => onConnected.Value = true)
                .AddTo(Disposables);

            VivoxClient.OnUserDisconnected
                .Where(participant => participant.IsSelf)
                .Subscribe(_ => onConnected.Value = false)
                .AddTo(Disposables);
        }

        public async UniTaskVoid JoinAsync()
            => ChannelId = await VivoxClient.ConnectAsync(CreateChannelConfig(channelName), cts.Token);

        protected abstract VivoxChannelConfig CreateChannelConfig(string channelName);

        public void Leave()
        {
            if (ChannelId != null)
            {
                VivoxClient.Disconnect(ChannelId);
            }
        }

        protected override void ReleaseManagedResources()
        {
            cts.Cancel();
            cts.Dispose();
            Disposables.Dispose();
            onConnected.Dispose();
        }
    }
}
