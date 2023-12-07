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

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private PubSubMultiplayClient pubSubMultiplayClient;

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

            var redisMessagingTransport = new NativeRedisMessagingTransport(
                new RedisMessagingConfig("http://localhost:3030", new SocketIOOptions { EIO = EngineIO.V4 }));
            var messagingMultiplayTransport = new NativeMultiplayTransport(redisMessagingTransport);
            pubSubMultiplayClient.SetTransport(messagingMultiplayTransport);
            builder.RegisterComponent(pubSubMultiplayClient);

            var textChatClient = TextChatClientProvider.Provide(peerClient);
            builder.RegisterComponent(textChatClient);
            var voiceChatClient = VoiceChatClientProvider.Provide(peerClient, assetHelper.VoiceChatConfig);
            builder.RegisterComponent(voiceChatClient);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
