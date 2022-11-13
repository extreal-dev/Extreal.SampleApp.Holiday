namespace Extreal.SampleApp.Holiday.App
{
    using System.Threading;
    using Core.StageNavigation;
    using Cysharp.Threading.Tasks;
    using VContainer;
    using VContainer.Unity;

    public class AppPresenter : IAsyncStartable
    {
        [Inject] private readonly IStageNavigator<StageName> stageNavigator;

        public async UniTask StartAsync(CancellationToken cancellation)
            => await stageNavigator.ReplaceAsync(StageName.TitleScreen);
    }
}
