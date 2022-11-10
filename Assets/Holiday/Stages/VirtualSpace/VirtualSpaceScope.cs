namespace Extreal.SampleApp.Holiday.Stages.VirtualSpace
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class VirtualSpaceScope : LifetimeScope
    {
        [SerializeField] private VirtualSpaceView virtualSpaceView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(virtualSpaceView);

            builder.RegisterEntryPoint<VirtualSpacePresenter>();
        }
    }
}
