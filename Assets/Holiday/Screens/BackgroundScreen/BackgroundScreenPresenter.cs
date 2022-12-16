using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
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
            BackgroundScreenView backgroundScreenView,
            AppState appState
        ) : base(stageNavigator)
        {
            this.backgroundScreenView = backgroundScreenView;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            appState.OnNotificationReceived
                .Subscribe(_ => backgroundScreenView.Hide())
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables) =>
            backgroundScreenView.Hide();

        protected override void OnStageExiting(StageName stageName) => backgroundScreenView.Show();
    }
}
