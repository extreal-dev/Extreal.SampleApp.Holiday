namespace Extreal.SampleApp.Holiday.App
{
    using App;
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
            builder.Register<SceneTransitioner<StageName, SceneName>>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<AppPresenter>();
        }
    }
}
