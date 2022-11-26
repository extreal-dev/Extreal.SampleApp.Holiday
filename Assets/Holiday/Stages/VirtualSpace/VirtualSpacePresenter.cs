using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Stages.VirtualSpace
{
    public class VirtualSpacePresenter : IStartable
    {
        [Inject] private IStageNavigator<StageName> stageNavigator;
        [Inject] private VirtualSpaceView virtualSpaceView;

        public void Start() =>
            virtualSpaceView.OnBackButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionScreen).Forget();
            });
    }
}
