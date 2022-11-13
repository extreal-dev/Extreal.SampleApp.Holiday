namespace Extreal.SampleApp.Holiday.Stages.VirtualSpace
{
    using App;
    using Core.StageNavigation;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

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
