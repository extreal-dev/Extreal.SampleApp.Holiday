namespace Extreal.SampleApp.Holiday.App
{
    using Core.StageNavigation;
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
            builder.Register<StageNavigator<StageName, SceneName>>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
