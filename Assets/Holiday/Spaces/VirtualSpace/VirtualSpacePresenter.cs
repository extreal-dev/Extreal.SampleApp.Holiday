using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;

namespace Extreal.SampleApp.Holiday.Spaces.VirtualSpace
{
    public class VirtualSpacePresenter : StagePresenterBase
    {
        private readonly VirtualSpaceView virtualSpaceView;

        public VirtualSpacePresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            VirtualSpaceView virtualSpaceView) : base(stageNavigator) =>
            this.virtualSpaceView = virtualSpaceView;

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            virtualSpaceView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
