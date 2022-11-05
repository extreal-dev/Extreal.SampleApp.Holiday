namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class LoadingScreenScope : LifetimeScope
    {
        [SerializeField] private LoadingScreenView loadingScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(loadingScreenView);

            builder.RegisterEntryPoint<LoadingScreenPresenter>();
        }
    }
}
