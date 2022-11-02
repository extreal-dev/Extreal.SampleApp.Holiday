namespace Extreal.SampleApp.Holiday.Stages.RoomSelectionScreen
{
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer.Unity;

    public class RoomSelectionScreenPresenter : IStartable
    {
        private readonly ISceneTransitioner<StageName> sceneTransitioner;
        private readonly RoomSelectionScreenView roomSelectionScreenView;

        public RoomSelectionScreenPresenter(ISceneTransitioner<StageName> sceneTransitioner, RoomSelectionScreenView roomSelectionScreenView)
        {
            this.sceneTransitioner = sceneTransitioner;
            this.roomSelectionScreenView = roomSelectionScreenView;
        }

        public void Start() =>
            roomSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                sceneTransitioner.ReplaceAsync(StageName.VirtualRoom).Forget();
            });
    }
}
