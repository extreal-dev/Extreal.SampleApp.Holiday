namespace Extreal.SampleApp.Holiday.App
{
    using Core.SceneTransition;
    using Models;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class AppScope : LifetimeScope
    {
        [SerializeField] private StageConfig stageConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            // Models
            builder.Register<Player>(Lifetime.Singleton);

            // Stage Config
            builder.RegisterInstance(stageConfig).AsImplementedInterfaces();
            builder.Register<SceneTransitioner<StageName, SceneName>>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StageNavigator>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
