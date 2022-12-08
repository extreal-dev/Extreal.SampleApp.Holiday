using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.ErrorScreen
{
    public class ErrorScreenScope : LifetimeScope
    {
        [SerializeField] private ErrorScreenView errorScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(errorScreenView);

            builder.RegisterEntryPoint<ErrorScreenPresenter>();
        }
    }
}
