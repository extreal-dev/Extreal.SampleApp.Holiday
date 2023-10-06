using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.PanoramicImageSpace
{
    public class PanoramicImageSpaceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<PanoramicImageSpacePresenter>();
    }
}
