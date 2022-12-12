using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MultiplayRoom>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MultiplayControlPresenter>();
        }
    }
}
