using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.P2PControl
{
    public class P2PControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<P2PControlPresenter>();
    }
}
