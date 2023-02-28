using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.TitleScreen
{
    public class TitleScreenPresenter : StagePresenterBase
    {
        private readonly TitleScreenView titleScreenView;
        private readonly AssetDownloadHelper assetDownloadHelper;

        public TitleScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            TitleScreenView titleScreenView,
            AssetDownloadHelper assetDownloadHelper
        ) : base(stageNavigator)
        {
            this.titleScreenView = titleScreenView;
            this.assetDownloadHelper = assetDownloadHelper;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            titleScreenView.OnGoButtonClicked
                .Subscribe(_ =>
                {
                    const string commonAssetName = nameof(AppConfigRepository);
                    assetDownloadHelper.DownloadAsync(commonAssetName, StageName.AvatarSelectionStage).Forget();
                })
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
