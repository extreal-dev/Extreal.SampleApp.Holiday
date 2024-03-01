using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.SfuControl
{
    public class SfuControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<SfuControlPresenter>();
    }
}
