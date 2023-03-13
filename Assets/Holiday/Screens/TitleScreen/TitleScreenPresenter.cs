using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.TitleScreen
{
    public class TitleScreenPresenter : StagePresenterBase
    {
        private readonly TitleScreenView titleScreenView;
        private readonly AssetHelper assetHelper;

        public TitleScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            TitleScreenView titleScreenView,
            AssetHelper assetHelper
        ) : base(stageNavigator)
        {
            this.titleScreenView = titleScreenView;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            titleScreenView.OnGoButtonClicked
                .Subscribe(_ => assetHelper.DownloadCommonAssetAsync(StageName.AvatarSelectionStage))
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
