namespace Extreal.SampleApp.Holiday.Stages.VirtualSpace
{
    using App;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class VirtualSpacePresenter : IStartable
    {
        [Inject] private StageNavigator stageNavigator;
        [Inject] private VirtualSpaceView virtualSpaceView;

        public void Start() =>
            virtualSpaceView.OnBackButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionScreen).Forget();
            });
    }
}
