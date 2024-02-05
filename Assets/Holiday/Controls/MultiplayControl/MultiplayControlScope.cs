using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        [SerializeField] private GameObject playerPrefab;

        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<MultiplayControlPresenter>().WithParameter(playerPrefab);
    }
}
