using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.Controls.TextChatControl;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.Controls.Common
{
    public abstract class ChatChannelBase : DisposableBase
    {
        public IObservable<bool> OnConnected => onConnected;

        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty onConnected = new BoolReactiveProperty(false);

        public IObservable<Unit> OnUnexpectedDisconnected
            => vivoxClient.OnRecoveryStateChanged
                .Where(recoveryState => recoveryState == ConnectionRecoveryState.FailedToRecover)
                .Select(_ => Unit.Default);

        public IObservable<Unit> OnConnectFailed => onConnectFailed;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onConnectFailed = new Subject<Unit>();

        private readonly VivoxClient vivoxClient;
        private readonly string channelName;

        protected ChannelId ChannelId { get; private set; }

        [SuppressMessage("Usage", "CC0022")]
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        protected ChatChannelBase(VivoxClient vivoxClient, string channelName)
        {
            this.vivoxClient = vivoxClient;
            this.channelName = channelName;

            vivoxClient.OnUserConnected
                .Where(participant => participant.IsSelf)
                .Subscribe(_ => onConnected.Value = true)
                .AddTo(Disposables);

            vivoxClient.OnUserDisconnected
                .Where(participant => participant.IsSelf)
                .Subscribe(_ => onConnected.Value = false)
                .AddTo(Disposables);
        }

        public async UniTask JoinAsync()
        {
            if (!IsLoggedIn)
            {
                try
                {
                    await vivoxClient.LoginAsync(new VivoxAuthConfig(nameof(TextChatChannel)));
                }
                catch (TimeoutException)
                {
                    onConnectFailed.OnNext(Unit.Default);
                    return;
                }
            }

            await UniTask.WaitUntil(() => IsLoggedIn, cancellationToken: cts.Token);

            try
            {
                ChannelId = await ConnectAsync(channelName);
            }
            catch (TimeoutException)
            {
                onConnectFailed.OnNext(Unit.Default);
            }
        }

        protected bool IsLoggedIn
            => vivoxClient.LoginSession?.State == LoginState.LoggedIn;

        protected abstract UniTask<ChannelId> ConnectAsync(string channelName);

        public void Leave()
        {
            if (!IsLoggedIn)
            {
                return;
            }

            vivoxClient.Disconnect(ChannelId);
        }

        protected override void ReleaseManagedResources()
        {
            cts.Cancel();
            cts.Dispose();
            Disposables.Dispose();
            onConnected.Dispose();
            onConnectFailed.Dispose();
        }
    }
}
