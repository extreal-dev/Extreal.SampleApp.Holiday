using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Data;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.TitleScreen
{
    public class TitleScreenPresenter : StagePresenterBase
    {
        private readonly TitleScreenView titleScreenView;
        private readonly DataRepository dataRepository;

        public TitleScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            TitleScreenView titleScreenView,
            DataRepository dataRepository
        ) : base(stageNavigator)
        {
            this.titleScreenView = titleScreenView;
            this.dataRepository = dataRepository;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            titleScreenView.OnGoButtonClicked
                .Subscribe(_ => dataRepository.GetDownloadSizeAsync().Forget())
                .AddTo(sceneDisposables);

            dataRepository.OnLoaded
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
