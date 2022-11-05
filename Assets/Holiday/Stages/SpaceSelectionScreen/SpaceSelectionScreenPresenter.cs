namespace Extreal.SampleApp.Holiday.Stages.RoomSelectionScreen
{
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class SpaceSelectionScreenPresenter : IStartable
    {
        [Inject]
        private readonly StageNavigator stageNavigator;

        [Inject]
        private readonly SpaceSelectionScreenView spaceSelectionScreenView;

        public void Start() =>
            spaceSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.VirtualSpace).Forget();
            });
    }
}
