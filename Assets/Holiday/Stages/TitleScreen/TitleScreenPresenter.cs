namespace Extreal.SampleApp.Holiday.Scenes.TitlePage
{
    using App;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class TitleScreenPresenter : IStartable
    {
        [Inject] private StageNavigator stageNavigator;

        [Inject] private TitleScreenView titleScreenView;

        public void Start() =>
            titleScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionScreen).Forget();
            });
    }
}
