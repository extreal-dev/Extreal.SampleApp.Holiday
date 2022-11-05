namespace Extreal.SampleApp.Holiday.Stages.AvatarSelectionScreen
{
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class AvatarSelectionScreenPresenter : IStartable
    {
        [Inject]
        private readonly ISceneTransitioner<StageName> sceneTransitioner;

        [Inject]
        private readonly AvatarSelectionScreenView avatarSelectionScreenView;

        public void Start() =>
            avatarSelectionScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                sceneTransitioner.ReplaceAsync(StageName.SpaceSelectionScreen).Forget();
            });
    }
}
