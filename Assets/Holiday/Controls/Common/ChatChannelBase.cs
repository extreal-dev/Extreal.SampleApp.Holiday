using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.Controls.TextChatControl;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.Controls.Common
{
    public abstract class ChatChannelBase : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(ChatChannelBase));

        public IObservable<bool> OnConnected => onConnected;

        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty onConnected = new BoolReactiveProperty(false);

        public IObservable<Unit> OnUnexpectedDisconnected
            => vivoxClient.OnRecoveryStateChanged
                .Where(recoveryState => recoveryState == ConnectionRecoveryState.FailedToRecover)
                .Select(_ => Unit.Default);

        public IObservable<Unit> OnConnectFailed => onConnectFailed;
        [SuppressMessage("Usage", "CC0033")] private readonly Subject<Unit> onConnectFailed = new Subject<Unit>();

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

            async void OnConnected(ChannelId channelId)
            {
                ChannelId = channelId;
                var channelSession = vivoxClient.LoginSession.GetChannelSession(ChannelId);
                try
                {
                    await UniTask.WaitUntil(
                        () => channelSession.ChannelState == ConnectionState.Connected,
                        cancellationToken: cts.Token);
                }
                catch (Exception)
                {
                    return;
                }

                onConnected.Value = true;
            }

            vivoxClient.OnChannelSessionAdded
                .Where(channelId => channelId.Name == channelName)
                .Subscribe(OnConnected)
                .AddTo(Disposables);

            vivoxClient.OnChannelSessionRemoved
                .Where(channelId => channelId.Name == channelName)
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
                await ConnectAsync(channelName);
            }
            catch (TimeoutException)
            {
                onConnectFailed.OnNext(Unit.Default);
            }
        }

        protected bool IsLoggedIn
            => vivoxClient.LoginSession?.State == LoginState.LoggedIn;

        protected abstract UniTask ConnectAsync(string channelName);

        public void Leave()
        {
            if (!IsLoggedIn)
            {
                return;
            }

            vivoxClient.Disconnect(ChannelId);
        }

        protected override void FreeManagedResources()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(nameof(FreeManagedResources));
            }

            cts.Cancel();
            cts.Dispose();
            Disposables.Dispose();
            onConnected.Dispose();
            onConnectFailed.Dispose();
        }
    }
}
