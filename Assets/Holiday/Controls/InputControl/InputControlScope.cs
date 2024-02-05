using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.InputControl
{
    public class InputControlScope : LifetimeScope
    {
        [SerializeField] private InputControlView inputControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(inputControlView);

            builder.RegisterEntryPoint<InputControlPresenter>();
        }
    }
}
