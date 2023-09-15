using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
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
            AppState appState,
            TitleScreenView titleScreenView,
            AssetHelper assetHelper
        ) : base(stageNavigator, appState)
        {
            this.titleScreenView = titleScreenView;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
            => titleScreenView.OnGoButtonClicked
                .Subscribe(_ => assetHelper.DownloadCommonAssetAsync(StageName.AvatarSelectionStage))
                .AddTo(sceneDisposables);
    }
}
