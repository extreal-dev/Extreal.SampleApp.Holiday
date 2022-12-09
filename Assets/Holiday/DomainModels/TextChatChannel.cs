using System.Threading;
using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Chat.Vivox;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.DomainModels
{
    public class TextChatChannel : IDisposable
    {
        public IObservable<bool> InText => inText;
        private readonly BoolReactiveProperty inText = new BoolReactiveProperty(false);
        public IObservable<string> OnTextMessageReceived
            => vivoxClient.OnTextMessageReceived.Select(channelTextMessage => channelTextMessage.Message);
        public IObservable<Unit> OnUnexpectedDisconnected
            => vivoxClient.OnRecoveryStateChanged
                .Where(recoveryState => recoveryState == ConnectionRecoveryState.FailedToRecover)
                .Select(_ => Unit.Default);

        private readonly VivoxClient vivoxClient;

        private string channelName;
        private ChannelId channelId;
        private bool isRecovering;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private IDisposable joinDisposable;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(TextChatChannel));

        public TextChatChannel(VivoxClient vivoxClient)
            => this.vivoxClient = vivoxClient;

        public void Initialize()
        {
            vivoxClient.OnChannelSessionAdded
                .Where(channelId => channelId.Name == channelName)
                .Subscribe(async channelId =>
                {
                    this.channelId = channelId;

                    var channelSession = vivoxClient.LoginSession.ChannelSessions[channelId];

                    try
                    {
                        await UniTask
                            .WaitUntil(
                                () => channelSession.TextState == ConnectionState.Connected,
                                cancellationToken: cts.Token);
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    inText.Value = true;
                })
                .AddTo(disposables);

            vivoxClient.OnChannelSessionRemoved
                .Where(channelId => channelId.Name == channelName)
                .Subscribe(_ =>
                {
                    channelName = string.Empty;
                    channelId = null;
                    inText.Value = false;
                })
                .AddTo(disposables);

            vivoxClient.OnRecoveryStateChanged
                .Where(recoveryState => recoveryState == ConnectionRecoveryState.Recovering)
                .Subscribe(_ => isRecovering = true)
                .AddTo(disposables);

            vivoxClient.OnRecoveryStateChanged
                .Where(recoveryState => recoveryState == ConnectionRecoveryState.Recovered)
                .Subscribe(_ => isRecovering = false)
                .AddTo(disposables);
        }

        public void Dispose()
        {
            cts.Cancel();

            cts.Dispose();
            inText.Dispose();
            joinDisposable?.Dispose();
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public void SetChannelName(string channelName)
            => this.channelName = channelName;

        public void Login()
        {
            if (vivoxClient.LoginSession == null || vivoxClient.LoginSession.State == LoginState.LoggedOut)
            {
                var authConfig = new VivoxAuthConfig(nameof(Holiday));
                vivoxClient.Login(authConfig);
            }
        }

        public void Join()
        {
            if (vivoxClient.LoginSession != null && vivoxClient.LoginSession.State == LoginState.LoggedIn)
            {
                JoinInternalAsync().Forget();
            }
            else
            {
                joinDisposable = vivoxClient.OnLoggedIn.Subscribe(_ => JoinInternalAsync().Forget());
            }
        }

        public void Leave()
        {
            joinDisposable?.Dispose();

            if (!ChannelId.IsNullOrEmpty(channelId) && !isRecovering)
            {
                vivoxClient.Disconnect(channelId);
            }

        }

        public void SendTextMessage(string message)
            => vivoxClient.SendTextMessage(message, channelId);

        private async UniTaskVoid JoinInternalAsync()
        {
            var channelConfig = new VivoxChannelConfig(channelName, ChatType.TextOnly, transmissionSwitch: false);
            vivoxClient.Connect(channelConfig);

            await UniTask.Yield();
            if (joinDisposable != null)
            {
                joinDisposable.Dispose();
                joinDisposable = null;
            }
        }
    }
}
