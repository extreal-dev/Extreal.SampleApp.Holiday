namespace Extreal.SampleApp.Holiday.Scenes.TitlePage
{
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class TitleScreenPresenter : IStartable
    {
        [Inject]
        private readonly StageNavigator stageNavigator;

        [Inject]
        private readonly TitleScreenView titleScreenView;

        public void Start() =>
            titleScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionScreen).Forget();
            });
    }
}
