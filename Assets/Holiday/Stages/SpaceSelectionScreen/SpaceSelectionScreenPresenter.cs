namespace Extreal.SampleApp.Holiday.Stages.RoomSelectionScreen
{
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer.Unity;

    public class SpaceSelectionScreenPresenter : IStartable
    {
        private readonly ISceneTransitioner<StageName> sceneTransitioner;
        private readonly SpaceSelectionScreenView spaceSelectionScreenView;

        public SpaceSelectionScreenPresenter(ISceneTransitioner<StageName> sceneTransitioner, SpaceSelectionScreenView spaceSelectionScreenView)
        {
            this.sceneTransitioner = sceneTransitioner;
            this.spaceSelectionScreenView = spaceSelectionScreenView;
        }

        public void Start() =>
            spaceSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                sceneTransitioner.ReplaceAsync(StageName.VirtualSpace).Forget();
            });
    }
}
