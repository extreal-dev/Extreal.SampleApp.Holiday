using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.TitleScreen
{
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
