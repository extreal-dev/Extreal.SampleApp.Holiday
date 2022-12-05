using Extreal.Integration.Multiplay.NGO;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Controls.MultiplayControl
{
    public class MultiplayControlScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(networkManager);
            builder.Register<NgoClient>(Lifetime.Singleton);
        }
    }
}
