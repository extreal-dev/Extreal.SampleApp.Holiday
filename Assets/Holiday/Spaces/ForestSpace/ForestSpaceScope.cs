using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.ForestSpace
{
    public class ForestSpaceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<ForestSpacePresenter>();
    }
}
