using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl
{
    public class LandscapeControlPresenter : StagePresenterBase
    {
        private readonly LandscapePlayerManager landscapePlayerManager;
        private readonly LandscapeControlView landscapeControlView;
        private readonly AssetHelper assetHelper;

        public LandscapeControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            LandscapePlayerManager landscapePlayerManager,
            LandscapeControlView landscapeControlView,
            AppState appState,
            AssetHelper assetHelper
        ) : base(stageNavigator, appState)
        {
            this.landscapePlayerManager = landscapePlayerManager;
            this.landscapeControlView = landscapeControlView;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables
        )
        {
        }

        protected override void OnStageEntered
        (
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables
        )
        {
            var isLandscapeTypeNone = appState.Space.LandscapeType == LandscapeType.None;
            landscapeControlView.SetStageActive(!isLandscapeTypeNone);
            PlayLandscapeAsync(stageName, appState, stageDisposables).Forget();
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            var isLandscapeTypeNone = appState.Space.LandscapeType == LandscapeType.None;
            landscapeControlView.SetStageActive(isLandscapeTypeNone);
            appState.SetLandscapeInitialized(false);
        }

        private async UniTaskVoid PlayLandscapeAsync
        (
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables
        )
        {
            var landscapePlayer = await landscapePlayerManager.CreateAsync(stageName, appState.Space.LandscapeType);
            landscapePlayer.OnErrorOccurred
                .Subscribe(_ => appState.Notify(assetHelper.MessageConfig.LandscapeErrorMessage))
                .AddTo(stageDisposables);

            landscapePlayer.AddTo(stageDisposables);

            landscapePlayer.Play();
        }
    }
}