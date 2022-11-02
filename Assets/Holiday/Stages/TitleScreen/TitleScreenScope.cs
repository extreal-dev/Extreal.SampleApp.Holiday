namespace Extreal.SampleApp.Holiday.Scenes.TitlePage
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class TitleScreenScope : LifetimeScope
    {
        [SerializeField] private TitleScreenView titleScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(titleScreenView);

            builder.RegisterEntryPoint<TitleScreenPresenter>();
        }
    }
}
