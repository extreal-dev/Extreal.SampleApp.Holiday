using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlPresenter : StagePresenterBase
    {
        private readonly VivoxClient vivoxClient;
        private readonly VoiceChatControlView voiceChatScreenView;

        private VoiceChatChannel voiceChatChannel;

        public VoiceChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            VivoxClient vivoxClient,
            VoiceChatControlView voiceChatScreenView
        ) : base(stageNavigator, appState)
        {
            this.vivoxClient = vivoxClient;
            this.voiceChatScreenView = voiceChatScreenView;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
            => voiceChatScreenView.OnMuteButtonClicked
                .Subscribe(_ => voiceChatChannel.ToggleMuteAsync().Forget())
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
            voiceChatChannel = new VoiceChatChannel(vivoxClient, $"HolidayVoiceChat{stageName}");
            stageDisposables.Add(voiceChatChannel);

            voiceChatChannel.OnConnected
                .Subscribe(appState.SetVoiceChatReady)
                .AddTo(stageDisposables);

            voiceChatChannel.OnMuted
                .Subscribe(voiceChatScreenView.ToggleMute)
                .AddTo(stageDisposables);

            voiceChatChannel.JoinAsync().Forget();
        }

        protected override void OnStageExiting(
            StageName stageName,
            AppState appState)
        {
            appState.SetVoiceChatReady(false);
            voiceChatChannel.Leave();
        }
    }
}
