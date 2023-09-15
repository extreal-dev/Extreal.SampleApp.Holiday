using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.BackgroundScreen
{
    public class BackgroundScreenPresenter : StagePresenterBase
    {
        private readonly BackgroundScreenView backgroundScreenView;
        private readonly AppState appState;

        public BackgroundScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            BackgroundScreenView backgroundScreenView
        ) : base(stageNavigator, appState)
            => this.backgroundScreenView = backgroundScreenView;

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
            => appState.OnNotificationReceived
                .Subscribe(_ => backgroundScreenView.Hide())
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
            => backgroundScreenView.Hide();

        protected override void OnStageExiting(
            StageName stageName,
            AppState appState)
            => backgroundScreenView.Show();
    }
}
