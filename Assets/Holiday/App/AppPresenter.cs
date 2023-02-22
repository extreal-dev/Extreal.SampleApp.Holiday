using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Config;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppPresenter : IAsyncStartable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        public AppPresenter(StageNavigator<StageName, SceneName> stageNavigator)
            => this.stageNavigator = stageNavigator;

        public async UniTask StartAsync(CancellationToken cancellation)
            => await stageNavigator.ReplaceAsync(StageName.TitleStage);
    }
}
