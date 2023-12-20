using Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl.Client;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl
{
    public class MassivelyMultiplayControlScope : LifetimeScope
    {
        [SerializeField] private GameObject playerPrefab;

        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<MassivelyMultiplayClientPresenter>().WithParameter(playerPrefab);
    }
}
