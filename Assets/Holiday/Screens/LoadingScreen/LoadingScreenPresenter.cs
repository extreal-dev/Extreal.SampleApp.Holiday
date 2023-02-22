using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Data;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenPresenter : StagePresenterBase
    {
        private readonly LoadingScreenView loadingScreenView;
        private readonly DataRepository dataRepository;
        private readonly AppState appState;

        public LoadingScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            LoadingScreenView loadingScreenView,
            DataRepository dataRepository,
            AppState appState
        ) : base(stageNavigator)
        {
            this.loadingScreenView = loadingScreenView;
            this.dataRepository = dataRepository;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            appState.IsLoading
                .Subscribe(OnLoadingChanged)
                .AddTo(sceneDisposables);

            dataRepository.LoadedPercent
                .Subscribe(loadingScreenView.SetLoadedPercent)
                .AddTo(sceneDisposables);

            appState.OnNotificationReceived
                .Subscribe(_ => loadingScreenView.Hide())
                .AddTo(sceneDisposables);
        }

        private void OnLoadingChanged(bool isLoading)
        {
            if (isLoading)
            {
                loadingScreenView.Show();
            }
            else
            {
                loadingScreenView.Hide();
            }
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            if (AppUtils.IsSpace(stageName))
            {
                appState.SetIsLoading(true);
            }
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
