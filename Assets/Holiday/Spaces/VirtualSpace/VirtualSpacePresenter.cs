using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.VirtualSpace
{
    public class VirtualSpacePresenter : IStartable
    {
        private readonly IStageNavigator<StageName> stageNavigator;
        private readonly VirtualSpaceView virtualSpaceView;

        public VirtualSpacePresenter(IStageNavigator<StageName> stageNavigator, VirtualSpaceView virtualSpaceView)
        {
            this.stageNavigator = stageNavigator;
            this.virtualSpaceView = virtualSpaceView;
        }

        public void Start() =>
            virtualSpaceView.OnBackButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget();
            });
    }
}
