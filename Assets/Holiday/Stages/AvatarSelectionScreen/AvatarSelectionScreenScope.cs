namespace Extreal.SampleApp.Holiday.Stages.AvatarSelectionScreen
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class AvatarSelectionScreenScope : LifetimeScope
    {
        [SerializeField] private AvatarSelectionScreenView avatarSelectionScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(avatarSelectionScreenView);

            builder.RegisterEntryPoint<AvatarSelectionScreenPresenter>();
        }
    }
}
