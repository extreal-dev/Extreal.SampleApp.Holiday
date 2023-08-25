using Extreal.SampleApp.Holiday.Controls.MultiplayControl.Host;
using Extreal.SampleApp.Holiday.Controls.MultiplyControl.Client;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplyControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        [SerializeField] private GameObject playerPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MultiplayHostPresenter>().WithParameter(playerPrefab);
            builder.RegisterEntryPoint<MultiplayClientPresenter>();
        }
    }
}
