using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppPresenter : IInitializable, IAsyncStartable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly VivoxClient vivoxClient;

        public AppPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            VivoxClient vivoxClient
        )
        {
            this.stageNavigator = stageNavigator;
            this.vivoxClient = vivoxClient;
        }

        public void Initialize()
        {
            var authConfig = new VivoxAuthConfig(nameof(Holiday));
            vivoxClient.Login(authConfig);
        }

        public async UniTask StartAsync(CancellationToken cancellation)
            => await stageNavigator.ReplaceAsync(StageName.TitleStage);
    }
}
