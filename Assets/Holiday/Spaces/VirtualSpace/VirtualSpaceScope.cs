using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.VirtualSpace
{
    public class VirtualSpaceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<VirtualSpacePresenter>();
    }
}
