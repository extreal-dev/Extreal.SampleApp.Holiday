using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.CableRailwaySpace
{
    public class CableRailwaySpaceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<CableRailwaySpacePresenter>();
    }
}
