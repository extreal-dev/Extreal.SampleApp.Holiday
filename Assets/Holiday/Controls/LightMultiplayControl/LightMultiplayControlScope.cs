using Extreal.SampleApp.Holiday.Controls.MultiplayControl.Host;
using Extreal.SampleApp.Holiday.Controls.LightMultiplyControl.Client;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.LightMultiplyControl
{
    public class LightMultiplayControlScope : LifetimeScope
    {
        [SerializeField] private GameObject playerPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LightMultiplayHostPresenter>().WithParameter(playerPrefab);
            builder.RegisterEntryPoint<LightMultiplayClientPresenter>();
        }
    }
}
