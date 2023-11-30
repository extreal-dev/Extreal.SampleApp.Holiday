using Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl.Client;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl
{
    public class MassivelyMultiplayControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<MassivelyMultiplayClientPresenter>();
    }
}
