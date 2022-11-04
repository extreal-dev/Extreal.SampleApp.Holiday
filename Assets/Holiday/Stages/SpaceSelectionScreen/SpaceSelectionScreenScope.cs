namespace Extreal.SampleApp.Holiday.Stages.RoomSelectionScreen
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class SpaceSelectionScreenScope : LifetimeScope
    {
        [SerializeField] private SpaceSelectionScreenView spaceSelectionScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(spaceSelectionScreenView);

            builder.RegisterEntryPoint<SpaceSelectionScreenPresenter>();
        }
    }
}
