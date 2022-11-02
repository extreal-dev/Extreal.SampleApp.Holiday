namespace Extreal.SampleApp.Holiday.Stages.AvatarSelectionScreen
{
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer.Unity;

    public class AvatarSelectionScreenPresenter : IStartable
    {
        private readonly ISceneTransitioner<StageName> sceneTransitioner;
        private readonly AvatarSelectionScreenView avatarSelectionScreenView;

        public AvatarSelectionScreenPresenter(ISceneTransitioner<StageName> sceneTransitioner, AvatarSelectionScreenView avatarSelectionScreenView)
        {
            this.sceneTransitioner = sceneTransitioner;
            this.avatarSelectionScreenView = avatarSelectionScreenView;
        }

        public void Start() =>
            avatarSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                sceneTransitioner.ReplaceAsync(StageName.RoomSelectionScreen).Forget();
            });
    }
}
