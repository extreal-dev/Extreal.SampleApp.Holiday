using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayHostControl
{
    public class MultiplayHostControlScope : LifetimeScope
    {
        [SerializeField] private GameObject playerPrefab;

        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<MultiplayHostControlPresenter>().WithParameter(playerPrefab);
    }
}
