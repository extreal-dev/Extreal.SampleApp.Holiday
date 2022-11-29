using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.TitleScreen
{
    public class TitleScreenPresenter : IStartable
    {
        private readonly IStageNavigator<StageName> stageNavigator;

        private readonly TitleScreenView titleScreenView;

        public TitleScreenPresenter(IStageNavigator<StageName> stageNavigator, TitleScreenView titleScreenView)
        {
            this.stageNavigator = stageNavigator;
            this.titleScreenView = titleScreenView;
        }

        public void Start() =>
            titleScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget();
            });
    }
}
