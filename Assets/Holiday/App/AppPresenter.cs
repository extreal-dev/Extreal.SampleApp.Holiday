using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppPresenter : IAsyncStartable
    {
        private readonly IStageNavigator<StageName> stageNavigator;

        public AppPresenter(IStageNavigator<StageName> stageNavigator) => this.stageNavigator = stageNavigator;

        public async UniTask StartAsync(CancellationToken cancellation)
            => await stageNavigator.ReplaceAsync(StageName.TitleScreen);
    }
}
