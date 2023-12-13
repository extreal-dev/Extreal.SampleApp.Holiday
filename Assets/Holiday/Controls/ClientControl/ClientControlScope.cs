using Extreal.Integration.Chat.WebRTC;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.Integration.Multiplay.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Unity.Netcode;
using Extreal.Integration.Multiplay.NGO;
using Extreal.Integration.Multiplay.NGO.WebRTC;
using Extreal.Integration.Messaging.Redis;
using SocketIOClient;
using Extreal.Integration.Messaging.Common;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private MultiplayClient multiplayClient;

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

            var ngoClient = new NgoClient(networkManager);
            ngoClient.AddConnectionSetter(webRtcTransportConnectionSetter);
            builder.RegisterComponent(ngoClient);

            var redisMessagingConfig = new RedisMessagingConfig("http://localhost:3030", new SocketIOOptions { EIO = EngineIO.V4 });
            var redisMessagingTransport = RedisMessagingTransportProvider.Provide(redisMessagingConfig);

            var messagingGroupManager = new Integration.Messaging.Common.GroupManager();
            messagingGroupManager.SetTransport(redisMessagingTransport);
            builder.RegisterComponent(messagingGroupManager);

            var messagingClient = new MessagingClient();
            messagingClient.SetTransport(redisMessagingTransport);
            var queuingMessagingClient = new QueuingMessagingClient(messagingClient);
            multiplayClient.SetMessagingClient(queuingMessagingClient);
            builder.RegisterComponent(multiplayClient);

            var textChatClient = TextChatClientProvider.Provide(peerClient);
            builder.RegisterComponent(textChatClient);
            var voiceChatClient = VoiceChatClientProvider.Provide(peerClient, assetHelper.VoiceChatConfig);
            builder.RegisterComponent(voiceChatClient);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
