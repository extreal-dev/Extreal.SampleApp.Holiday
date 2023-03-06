using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.Common;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        [SerializeField]
        private NetworkManager networkManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(networkManager);
            builder.Register<NgoClient>(Lifetime.Singleton);

            var assetHelper = Parent.Container.Resolve<AssetHelper>();
            builder.RegisterComponent(assetHelper.VivoxAppConfig);
            builder.Register<VivoxClient>(Lifetime.Singleton);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
