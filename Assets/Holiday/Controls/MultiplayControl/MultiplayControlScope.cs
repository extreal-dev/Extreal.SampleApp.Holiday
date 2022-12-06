using Extreal.SampleApp.Holiday.Models;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MultiplayRoom>().AsSelf();
            builder.RegisterEntryPoint<MultiplayControlPresenter>();
        }
    }
}
