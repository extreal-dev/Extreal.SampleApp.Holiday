using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.ForestSpace
{
    public class PanoramicImageSpaceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<PanoramicImageSpacePresenter>();
    }
}