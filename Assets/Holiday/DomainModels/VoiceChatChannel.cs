using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Chat.Vivox;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.DomainModels
{
    public class VoiceChatChannel : IDisposable
    {
        public IObservable<bool> InAudio => inAudio;
        private readonly BoolReactiveProperty inAudio = new BoolReactiveProperty(false);

        public IObservable<string> OnMuted => onMuted;
        private readonly ReactiveProperty<string> onMuted = new ReactiveProperty<string>();

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

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(VoiceChatChannel));

        public VoiceChatChannel(VivoxClient vivoxClient)
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
                                () => channelSession.AudioState == ConnectionState.Connected,
                                cancellationToken: cts.Token);
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    inAudio.Value = true;
                })
                .AddTo(disposables);

            vivoxClient.OnChannelSessionRemoved
                .Where(channelId => channelId.Name == channelName)
                .Subscribe(_ =>
                {
                    channelName = string.Empty;
                    channelId = null;
                    inAudio.Value = false;
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
            inAudio.Dispose();
            onMuted.Dispose();
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

        private async UniTaskVoid JoinInternalAsync()
        {
            var audioInputDevices = await vivoxClient.GetAudioInputDevicesAsync();
            audioInputDevices.Muted = true;
            onMuted.Value = "OFF";

            var channelConfig = new VivoxChannelConfig(channelName, ChatType.AudioOnly);
            vivoxClient.Connect(channelConfig);

            if (joinDisposable != null)
            {
                await UniTask.Yield();
                joinDisposable.Dispose();
                joinDisposable = null;
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

        public async UniTask ToggleMuteAsync()
        {
            var audioInputDevices = await vivoxClient.GetAudioInputDevicesAsync();
            audioInputDevices.Muted ^= true;
            onMuted.Value = audioInputDevices.Muted ? "OFF" : "ON";
        }
    }
}
