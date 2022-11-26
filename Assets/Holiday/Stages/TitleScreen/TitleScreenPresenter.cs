using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Scenes.TitlePage
{
    public class TitleScreenPresenter : IStartable
    {
        [Inject] private IStageNavigator<StageName> stageNavigator;

        [Inject] private TitleScreenView titleScreenView;

        public void Start() =>
            titleScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionScreen).Forget();
            });
    }
}
