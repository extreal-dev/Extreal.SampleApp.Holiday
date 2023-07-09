using System;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.NGO.Dev;
using Extreal.NGO.WebRTC.Dev;
using Extreal.P2P.Dev;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
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

            var peerClient = PeerClientProvider.Provide(assetHelper.PeerConfig);
            builder.RegisterComponent(peerClient);

            builder.Register<GroupManager>(Lifetime.Singleton);

            builder.RegisterComponent(networkManager);
            var webRtcClient = WebRtcClientProvider.Provide(peerClient);
            builder.RegisterComponent(webRtcClient);
            builder.Register<WebRtcTransportConnectionSetter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<NgoHost>(Lifetime.Singleton);
            builder.Register<NgoClient>(Lifetime.Singleton).WithParameter(assetHelper.NgoClientConfig.RetryStrategy);

            builder.RegisterComponent(assetHelper.VivoxAppConfig);
            builder.Register<VivoxClient>(Lifetime.Singleton);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }

        private static PeerClient Hoge(IObjectResolver arg)
        {
            throw new NotImplementedException();
        }
    }
}
