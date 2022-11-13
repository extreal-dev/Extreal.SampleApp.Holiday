namespace Extreal.SampleApp.Holiday.Scenes.TitlePage
{
    using App;
    using Core.StageNavigation;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class TitleScreenPresenter : IStartable
    {
        [Inject] private IStageNavigator<StageName> stageNavigator;

        [Inject] private TitleScreenView titleScreenView;

        public void Start() =>
            titleScreenView.OnGoButtonClicked.Subscribe(_ =>
            {
                stageNavigator.ReplaceAsync(StageName.AvatarSelectionScreen).Forget();
            });
    }
}
