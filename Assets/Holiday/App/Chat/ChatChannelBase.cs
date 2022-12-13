using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.Controls.TextChatControl;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.App
{
    public abstract class ChatChannelBase : IDisposable
    {
        public IObservable<bool> OnConnected => onConnected;
        private readonly BoolReactiveProperty onConnected = new BoolReactiveProperty(false);

        public IObservable<Unit> OnUnexpectedDisconnected
            => vivoxClient.OnRecoveryStateChanged
                .Where(recoveryState => recoveryState == ConnectionRecoveryState.FailedToRecover)
                .Select(_ => Unit.Default);

        public IObservable<Unit> OnConnectFailed => onConnectFailed;
        private readonly Subject<Unit> onConnectFailed = new Subject<Unit>();

        private readonly VivoxClient vivoxClient;
        private readonly string channelName;

        protected ChannelId ChannelId;
        protected readonly CompositeDisposable Disposables = new CompositeDisposable();

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
                    await vivoxClient.Login(new VivoxAuthConfig(nameof(TextChatChannel)));
                }
                catch (TimeoutException)
                {
                    onConnectFailed.OnNext(Unit.Default);
                    return;
                }
            }

            await UniTask.WaitUntil(() => IsLoggedIn, cancellationToken: cts.Token);
            Connect(channelName);
        }

        protected bool IsLoggedIn
            => vivoxClient.LoginSession?.State == LoginState.LoggedIn;

        protected abstract void Connect(string channelName);

        public void Leave()
        {
            if (!IsLoggedIn)
            {
                return;
            }

            vivoxClient.Disconnect(ChannelId);
        }

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            Disposables.Dispose();
            onConnected.Dispose();
            onConnectFailed.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
