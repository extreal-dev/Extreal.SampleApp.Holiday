using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.OME;
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
        {
            voiceChatScreenView.OnMuteButtonClicked
                .Subscribe(_ => voiceChatClient.ToggleMute())
                .AddTo(sceneDisposables);

            voiceChatClient.OnMuted
                .Subscribe(voiceChatScreenView.ToggleMute)
                .AddTo(sceneDisposables);
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            if (AppUtils.IsSpace(stageName))
            {
                return;
            }
            voiceChatClient.Clear();
        }
    }
}
