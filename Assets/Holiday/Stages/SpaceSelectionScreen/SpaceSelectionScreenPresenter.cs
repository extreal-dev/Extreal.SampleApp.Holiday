namespace Extreal.SampleApp.Holiday.Stages.RoomSelectionScreen
{
    using App;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class SpaceSelectionScreenPresenter : IStartable
    {
        [Inject] private StageNavigator stageNavigator;

        [Inject] private SpaceSelectionScreenView spaceSelectionScreenView;

        public void Start() =>
            spaceSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.VirtualSpace).Forget();
            });
    }
}
