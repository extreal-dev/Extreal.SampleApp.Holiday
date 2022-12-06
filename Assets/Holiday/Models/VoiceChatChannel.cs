using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Chat.Vivox;
using UniRx;
using VContainer.Unity;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.Models
{
    public class VoiceChatChannel : IInitializable, IDisposable
    {
        public IObservable<bool> InAudio => inAudio;
        private readonly BoolReactiveProperty inAudio = new BoolReactiveProperty(false);

        public IObservable<string> OnMuted => onMuted;
        private readonly ReactiveProperty<string> onMuted = new ReactiveProperty<string>();

        private readonly VivoxClient vivoxClient;

        private string channelName;
        private ChannelId channelId;

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
        }

        public void Dispose()
        {
            cts.Cancel();

            cts.Dispose();
            inAudio.Dispose();
            onMuted.Dispose();
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Join(string channelName)
        {
            if (vivoxClient.LoginSession != null && vivoxClient.LoginSession.State == LoginState.LoggedIn)
            {
                JoinInternalAsync(channelName).Forget();
            }
            else
            {
                joinDisposable = vivoxClient.OnLoggedIn.Subscribe(_ => JoinInternalAsync(channelName).Forget());
            }
        }

        private async UniTaskVoid JoinInternalAsync(string channelName)
        {
            var audioInputDevices = await vivoxClient.GetAudioInputDevicesAsync();
            audioInputDevices.Muted = true;
            onMuted.Value = "OFF";

            this.channelName = channelName;
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
            => vivoxClient.Disconnect(channelId);

        public async UniTask ToggleMuteAsync()
        {
            var audioInputDevices = await vivoxClient.GetAudioInputDevicesAsync();
            audioInputDevices.Muted ^= true;
            onMuted.Value = audioInputDevices.Muted ? "OFF" : "ON";
        }
    }
}
