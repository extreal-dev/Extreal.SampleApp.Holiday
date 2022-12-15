using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlPresenter : StagePresenterBase
    {
        private readonly VivoxClient vivoxClient;
        private readonly VoiceChatControlView voiceChatScreenView;
        private readonly AppState appState;

        private VoiceChatChannel voiceChatChannel;

        public VoiceChatControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            VivoxClient vivoxClient,
            VoiceChatControlView voiceChatScreenView,
            AppState appState) : base(stageNavigator)
        {
            this.vivoxClient = vivoxClient;
            this.voiceChatScreenView = voiceChatScreenView;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            voiceChatScreenView.OnMuteButtonClicked
                .Subscribe(_ => voiceChatChannel.ToggleMuteAsync().Forget())
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            voiceChatChannel = new VoiceChatChannel(vivoxClient, $"HolidayVoiceChat{stageName}");

            voiceChatChannel.OnConnected
                .Subscribe(appState.SetInAudio)
                .AddTo(stageDisposables);

            voiceChatChannel.OnMuted
                .Subscribe(voiceChatScreenView.ToggleMute)
                .AddTo(stageDisposables);

            voiceChatChannel.OnUnexpectedDisconnected
                .Subscribe(_ => appState.SetNotification("Unexpected disconnection from vivox server has occurred"))
                .AddTo(stageDisposables);

            voiceChatChannel.OnConnectFailed
                .Subscribe(_ => appState.SetNotification("Connection to vivox server is failed"))
                .AddTo(stageDisposables);

            voiceChatChannel.JoinAsync().Forget();
        }

        protected override void OnStageExiting(StageName stageName)
        {
            appState.SetInAudio(false);
            voiceChatChannel.Leave();
        }
    }
}
