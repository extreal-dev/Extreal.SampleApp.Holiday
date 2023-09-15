using Extreal.Integration.Chat.WebRTC;
using Extreal.Integration.Multiplay.NGO;
using Extreal.Integration.Multiplay.NGO.WebRTC;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;

        protected override void Configure(IContainerBuilder builder)
        {
            var assetHelper = Parent.Container.Resolve<AssetHelper>();

            var peerClient = PeerClientProvider.Provide(assetHelper.PeerConfig);
            builder.RegisterComponent(peerClient);

            builder.Register<GroupManager>(Lifetime.Singleton);

            builder.RegisterComponent(networkManager);

            var webRtcClient = WebRtcClientProvider.Provide(peerClient);
            var webRtcTransportConnectionSetter = new WebRtcTransportConnectionSetter(webRtcClient);

            var ngoHost = new NgoServer(networkManager);
            ngoHost.AddConnectionSetter(webRtcTransportConnectionSetter);
            builder.RegisterComponent(ngoHost);

            var ngoClient = new NgoClient(networkManager, assetHelper.NgoClientConfig.RetryStrategy);
            ngoClient.AddConnectionSetter(webRtcTransportConnectionSetter);
            builder.RegisterComponent(ngoClient);

            var textChatClient = TextChatClientProvider.Provide(peerClient);
            builder.RegisterComponent(textChatClient);
            var voiceChatClient = VoiceChatClientProvider.Provide(peerClient, assetHelper.VoiceChatConfig);
            builder.RegisterComponent(voiceChatClient);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
