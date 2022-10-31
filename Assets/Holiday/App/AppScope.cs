namespace Extreal.SampleApp.Holiday.Main
{
    using Config;
    using Core.SceneTransition;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class AppScope : LifetimeScope
    {
        [SerializeField] private SceneConfig sceneConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(sceneConfig).AsImplementedInterfaces();
            builder.Register<SceneTransitioner<SceneName, UnitySceneName>>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
