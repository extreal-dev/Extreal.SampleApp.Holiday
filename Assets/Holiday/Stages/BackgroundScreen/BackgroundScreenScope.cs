namespace Extreal.SampleApp.Holiday.Stages.BackgroundScreen
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class BackgroundScreenScope : LifetimeScope
    {
        [SerializeField] private BackgroundScreenView backgroundScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(backgroundScreenView);

            builder.RegisterEntryPoint<BackgroundScreenPresenter>();
        }
    }
}
