namespace Extreal.SampleApp.Holiday.App
{
    using Core.SceneTransition;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class AppScope : LifetimeScope
    {
        [SerializeField] private StageConfig stageConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(stageConfig).AsImplementedInterfaces();
            builder.Register<SceneTransitioner<StageName, SceneName>>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StageNavigator>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
