using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.SpaceControl
{
    public class SpaceControlPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(SpaceControlPresenter));

        private readonly SpaceControlView spaceControlView;
        private readonly AppState appState;
        private readonly AssetHelper assetHelper;

        public SpaceControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            SpaceControlView spaceControlView,
            AppState appState,
            AssetHelper assetHelper
        ) : base(stageNavigator)
        {
            this.spaceControlView = spaceControlView;
            this.appState = appState;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            spaceControlView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
            => LoadSpaceAsync(appState.SpaceName.Value, stageDisposables).Forget();

        private async UniTaskVoid LoadSpaceAsync(string spaceName, CompositeDisposable stageDisposables)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Load space: {spaceName}");
            }
            var scene = await assetHelper.LoadSceneAsync(spaceName);
            stageDisposables.Add(scene);
            appState.SetSpaceReady(true);
        }

        protected override void OnStageExiting(StageName stageName) => appState.SetSpaceReady(false);
    }
}
