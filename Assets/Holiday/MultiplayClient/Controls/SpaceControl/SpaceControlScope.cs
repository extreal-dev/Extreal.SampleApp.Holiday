using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Controls.SpaceControl
{
    public class SpaceControlScope : LifetimeScope
    {
        [SerializeField] private SpaceControlView spaceControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(spaceControlView);
            builder.RegisterEntryPoint<Models.Space>(Lifetime.Singleton).AsSelf();

            builder.RegisterEntryPoint<SpaceControlPresenter>();
        }
    }
}
