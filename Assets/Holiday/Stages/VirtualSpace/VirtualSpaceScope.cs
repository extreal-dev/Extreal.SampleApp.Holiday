namespace Extreal.SampleApp.Holiday.Holiday.Stages.VirtualSpace
{
    using VContainer;
    using VContainer.Unity;

    public class VirtualSpaceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<VirtualSpaceProvider>();
        }
    }
}
