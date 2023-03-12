using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.RetryStatusControl
{
    public class RetryStatusControlScope : LifetimeScope
    {
        [SerializeField] private RetryStatusControlView retryStatusControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(retryStatusControlView);

            builder.RegisterEntryPoint<RetryStatusControlPresenter>();
        }
    }
}
