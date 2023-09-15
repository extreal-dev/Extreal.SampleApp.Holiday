using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenPresenter : StagePresenterBase
    {
        private readonly LoadingScreenView loadingScreenView;
        private readonly AssetHelper assetHelper;

        public LoadingScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            LoadingScreenView loadingScreenView,
            AssetHelper assetHelper
        ) : base(stageNavigator, appState)
        {
            this.loadingScreenView = loadingScreenView;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            appState.PlayingReady
                .Subscribe(ready => loadingScreenView.SwitchVisibility(!ready))
                .AddTo(sceneDisposables);

            appState.OnNotificationReceived
                .Subscribe(_ => loadingScreenView.SwitchVisibility(false))
                .AddTo(sceneDisposables);

            assetHelper.OnDownloading
                .Subscribe(_ => loadingScreenView.SwitchVisibility(true))
                .AddTo(sceneDisposables);

            assetHelper.OnDownloaded
                .Subscribe(loadingScreenView.SetDownloadStatus)
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
            if (!AppUtils.IsSpace(stageName))
            {
                loadingScreenView.SwitchVisibility(false);
            }
        }

        protected override void OnStageExiting(
            StageName stageName,
            AppState appState)
        {
            if (AppUtils.IsSpace(stageName))
            {
                loadingScreenView.SwitchVisibility(true);
            }
        }
    }
}
