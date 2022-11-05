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
        private readonly ISceneTransitioner<StageName> sceneTransitioner;

        [Inject]
        private readonly SpaceSelectionScreenView spaceSelectionScreenView;

        public void Start() =>
            spaceSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                sceneTransitioner.ReplaceAsync(StageName.VirtualSpace).Forget();
            });
    }
}
