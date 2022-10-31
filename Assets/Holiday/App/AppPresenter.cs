namespace Extreal.SampleApp.Holiday.Main
{
    using System.Threading;
    using Config;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using VContainer.Unity;

    public class AppPresenter : IAsyncStartable
    {
        private readonly ISceneTransitioner<SceneName> sceneTransitioner;

        public AppPresenter(ISceneTransitioner<SceneName> sceneTransitioner)
            => this.sceneTransitioner = sceneTransitioner;

        public async UniTask StartAsync(CancellationToken cancellation)
            => await sceneTransitioner.ReplaceAsync(SceneName.TitlePage);
    }
}
