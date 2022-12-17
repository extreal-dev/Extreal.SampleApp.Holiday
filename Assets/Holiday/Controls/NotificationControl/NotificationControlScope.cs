using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.NotificationControl
{
    public class NotificationControlScope : LifetimeScope
    {
        [SerializeField] private NotificationControlView notificationControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(notificationControlView);

            builder.RegisterEntryPoint<NotificationControlPresenter>();
        }
    }
}
