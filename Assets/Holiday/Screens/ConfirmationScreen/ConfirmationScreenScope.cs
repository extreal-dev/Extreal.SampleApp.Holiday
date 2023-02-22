using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.ConfirmationScreen
{
    public class ConfirmationScreenScope : LifetimeScope
    {
        [SerializeField] private ConfirmationScreenView confirmationScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(confirmationScreenView);

            builder.RegisterEntryPoint<ConfirmationScreenPresenter>();
        }
    }
}
