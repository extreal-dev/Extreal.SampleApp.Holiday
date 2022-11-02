namespace Extreal.SampleApp.Holiday.Scenes.TitlePage
{
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer.Unity;

    public class TitleScreenPresenter : IStartable
    {
        private readonly ISceneTransitioner<StageName> sceneTransitioner;
        private readonly TitleScreenView titleScreenView;

        public TitleScreenPresenter(ISceneTransitioner<StageName> sceneTransitioner, TitleScreenView titleScreenView)
        {
            this.sceneTransitioner = sceneTransitioner;
            this.titleScreenView = titleScreenView;
        }

        public void Start() =>
            titleScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                sceneTransitioner.ReplaceAsync(StageName.AvatarSelectionScreen).Forget();
            });
    }
}
