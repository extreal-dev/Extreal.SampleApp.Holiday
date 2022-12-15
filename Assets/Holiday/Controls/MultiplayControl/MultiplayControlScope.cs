using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        [SerializeField] private MultiplayConnectionConfig multiplayConnectionConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(multiplayConnectionConfig);
            builder.RegisterEntryPoint<MultiplayControlPresenter>();
        }
    }
}
