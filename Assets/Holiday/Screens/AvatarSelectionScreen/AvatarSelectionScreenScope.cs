using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
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
