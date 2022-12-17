using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenPresenter : StagePresenterBase
    {
        private readonly LoadingScreenView loadingScreenView;
        private readonly AppState appState;

        public LoadingScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            LoadingScreenView loadingScreenView,
            AppState appState
        ) : base(stageNavigator)
        {
            this.loadingScreenView = loadingScreenView;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            appState.IsPlaying
                .Subscribe(OnPlayingChanged)
                .AddTo(sceneDisposables);

            appState.OnNotificationReceived
                .Subscribe(_ => loadingScreenView.Hide())
                .AddTo(sceneDisposables);
        }

        private void OnPlayingChanged(bool isPlaying)
        {
            if (isPlaying)
            {
                loadingScreenView.Hide();
            }
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            if (AppUtils.IsSpace(stageName))
            {
                loadingScreenView.Show();
            }
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
