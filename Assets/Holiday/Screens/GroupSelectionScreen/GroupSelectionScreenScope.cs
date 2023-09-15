using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.GroupSelectionScreen
{
    public class GroupSelectionScreenScope : LifetimeScope
    {
        [SerializeField] private GroupSelectionScreenView groupSelectionScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(groupSelectionScreenView);

            builder.RegisterEntryPoint<GroupSelectionScreenPresenter>();
        }
    }
}
