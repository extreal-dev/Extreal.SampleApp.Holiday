using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.PanoramicVideoSpace
{
    public class PanoramicVideoSpaceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<PanoramicVideoSpacePresenter>();
    }
}
