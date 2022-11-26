using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    public class PlayerScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder) => builder.RegisterEntryPoint<PlayerPresenter>();
    }
}
