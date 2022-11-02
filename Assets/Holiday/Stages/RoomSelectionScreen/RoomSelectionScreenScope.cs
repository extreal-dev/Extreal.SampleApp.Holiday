namespace Extreal.SampleApp.Holiday.Stages.RoomSelectionScreen
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class RoomSelectionScreenScope : LifetimeScope
    {
        [SerializeField] private RoomSelectionScreenView roomSelectionScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(roomSelectionScreenView);

            builder.RegisterEntryPoint<RoomSelectionScreenPresenter>();
        }
    }
}
