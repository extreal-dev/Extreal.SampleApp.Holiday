using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Chat.Dev;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlPresenter : StagePresenterBase
    {
        private readonly VoiceChatClient voiceChatClient;
        private readonly VoiceChatControlView voiceChatScreenView;

        public VoiceChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            VoiceChatClient voiceChatClient,
            VoiceChatControlView voiceChatScreenView
        ) : base(stageNavigator, appState)
        {
            this.voiceChatClient = voiceChatClient;
            this.voiceChatScreenView = voiceChatScreenView;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
            => voiceChatScreenView.OnMuteButtonClicked
                .Subscribe(_ => voiceChatClient.ToggleMute())
                .AddTo(sceneDisposables);

        [SuppressMessage("CodeCracker", "CC0092")]
        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
            Observable
                .CombineLatest(appState.SpaceReady, appState.P2PReady)
                .Where(readies => readies.All(ready => ready))
                .Subscribe(_ => appState.SetVoiceChatReady(true))
                .AddTo(stageDisposables);

            voiceChatClient.OnMuted
                .Subscribe(voiceChatScreenView.ToggleMute)
                .AddTo(stageDisposables);
        }

        protected override void OnStageExiting(
            StageName stageName,
            AppState appState)
        {
            appState.SetVoiceChatReady(false);
            voiceChatClient.Clear();
        }
    }
}
