namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class PlayerScope : LifetimeScope
    {
        [SerializeField] private PlayerView playerView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerView);

            builder.RegisterEntryPoint<PlayerPresenter>();
        }
    }
}
