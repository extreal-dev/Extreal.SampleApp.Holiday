using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.VirtualSpaceControl
{
    public class VirtualSpaceControlScope : LifetimeScope
    {
        [SerializeField] private VirtualSpaceControlView virtualSpaceControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(virtualSpaceControlView);

            builder.RegisterEntryPoint<VirtualSpaceControlPresenter>();
        }
    }
}
