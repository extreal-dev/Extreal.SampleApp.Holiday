using Extreal.SampleApp.Holiday.MultiplayClient.Models;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Controls.MultiplayControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MultiplayRoom>().AsSelf();
            builder.RegisterEntryPoint<MultiplayPresenter>();
        }
    }
}
