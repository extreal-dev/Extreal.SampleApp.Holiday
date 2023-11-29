using Extreal.SampleApp.Holiday.Controls.MultiplyControl.Client;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplyControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MultiplayClientPresenter>();
        }
    }
}
