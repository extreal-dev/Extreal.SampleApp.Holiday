using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
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
            AppState appState,
            SpaceControlView spaceControlView,
            AssetHelper assetHelper
        ) : base(stageNavigator, appState)
        {
            this.spaceControlView = spaceControlView;
            this.appState = appState;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            spaceControlView.OnSpaceChanged
                .Subscribe(spaceName =>
                {
                    var space = assetHelper.SpaceConfig.Spaces.First(space => space.SpaceName == spaceName);
                    appState.SetSpace(space);
                })
                .AddTo(sceneDisposables);

            spaceControlView.OnGoButtonClicked
                .Subscribe(_ =>
                {
                    SwitchSpace(appState, stageNavigator);
                })
                .AddTo(sceneDisposables);

            spaceControlView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.GroupSelectionStage).Forget())
                .AddTo(sceneDisposables);

            InitializeView(appState);
        }


        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
        }

        private void InitializeView(AppState appState)
        {
            var spaces = assetHelper.SpaceConfig.Spaces;
            var spaceNames = spaces.Select(space => space.SpaceName).ToList();

            spaceControlView.Initialize(spaceNames);
            spaceControlView.SetSpaceDropdownValue(appState.Space.SpaceName);
        }

        private void SwitchSpace(AppState appState, StageNavigator<StageName, SceneName> stageNavigator)
        {
            var landscapeType = appState.Space.LandscapeType;
            if (landscapeType == LandscapeType.None)
            {
                assetHelper.DownloadSpaceAsset(appState.SpaceName, appState.Space.StageName);
            }
            if (landscapeType == LandscapeType.Image)
            {
                appState.SetSpace(assetHelper.SpaceConfig.Spaces.Find(space => space.SpaceName == "PanoramicImageSpace"));
                stageNavigator.ReplaceAsync(StageName.PanoramicImageStage).Forget();
            }
            if (landscapeType == LandscapeType.Video)
            {
                appState.SetSpace(assetHelper.SpaceConfig.Spaces.Find(space => space.SpaceName == "PanoramicVideoSpace"));
                stageNavigator.ReplaceAsync(StageName.PanoramicVideoStage).Forget();
            }
        }

        protected override void OnStageExiting(
            StageName stageName,
            AppState appState)
        { }
    }
}
