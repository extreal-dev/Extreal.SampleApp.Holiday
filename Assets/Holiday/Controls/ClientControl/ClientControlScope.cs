using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Extreal.NGO;
using Extreal.SampleApp.Holiday.App.P2P;
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
            var assetHelper = Parent.Container.Resolve<AssetHelper>();

            builder.Register<GroupManager>(Lifetime.Singleton);

            builder.RegisterComponent(networkManager);
            builder.Register<NgoClient>(Lifetime.Singleton).WithParameter(assetHelper.NgoClientConfig.RetryStrategy);
            builder.Register<NgoHost>(Lifetime.Singleton);

            builder.RegisterComponent(assetHelper.VivoxAppConfig);
            builder.Register<VivoxClient>(Lifetime.Singleton);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
