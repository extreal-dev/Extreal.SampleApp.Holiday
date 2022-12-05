using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.MultiplayClient.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Screens.TitleScreen
{
    public class TitleScreenPresenter : IStartable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        private readonly TitleScreenView titleScreenView;

        public TitleScreenPresenter(StageNavigator<StageName, SceneName> stageNavigator, TitleScreenView titleScreenView)
        {
            this.stageNavigator = stageNavigator;
            this.titleScreenView = titleScreenView;
        }

        public void Start() =>
            titleScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget());
    }
}
