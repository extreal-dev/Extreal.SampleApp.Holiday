using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlPresenter : StagePresenterBase
    {
        private readonly VivoxClient vivoxClient;
        private readonly VoiceChatControlView voiceChatScreenView;
        private readonly AppState appState;

        private VoiceChatChannel voiceChatChannel;

        public VoiceChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            VivoxClient vivoxClient,
            VoiceChatControlView voiceChatScreenView,
            AppState appState
        ) : base(stageNavigator)
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
            stageDisposables.Add(voiceChatChannel);

            voiceChatChannel.OnConnected
                .Subscribe(appState.SetVoiceChatReady)
                .AddTo(stageDisposables);

            voiceChatChannel.OnMuted
                .Subscribe(voiceChatScreenView.ToggleMute)
                .AddTo(stageDisposables);

            voiceChatChannel.JoinAsync().Forget();
        }

        protected override void OnStageExiting(StageName stageName)
        {
            appState.SetVoiceChatReady(false);
            voiceChatChannel.Leave();
        }
    }
}
