namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    using VContainer;
    using VContainer.Unity;

    public class PlayerScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder) => builder.RegisterEntryPoint<PlayerPresenter>();
    }
}
