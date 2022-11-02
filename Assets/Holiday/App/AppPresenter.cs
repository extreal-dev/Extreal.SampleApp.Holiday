namespace Extreal.SampleApp.Holiday.App
{
    using System.Threading;
    using App;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using VContainer.Unity;

    public class AppPresenter : IAsyncStartable
    {
        private readonly ISceneTransitioner<StageName> sceneTransitioner;

        public AppPresenter(ISceneTransitioner<StageName> sceneTransitioner)
            => this.sceneTransitioner = sceneTransitioner;

        public async UniTask StartAsync(CancellationToken cancellation)
            => await sceneTransitioner.ReplaceAsync(StageName.TitleScreen);
    }
}
