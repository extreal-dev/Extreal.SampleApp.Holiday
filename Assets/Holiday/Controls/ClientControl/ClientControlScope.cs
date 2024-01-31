using Extreal.Integration.Chat.WebRTC;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.Integration.Multiplay.Messaging;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Extreal.Integration.Messaging.Redis;
using Extreal.Integration.Messaging;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        [SerializeField] private NetworkObjectsProvider networkObjectsProvider;

        protected override void Configure(IContainerBuilder builder)
        {
            var assetHelper = Parent.Container.Resolve<AssetHelper>();

            var peerClient = PeerClientProvider.Provide(assetHelper.PeerConfig);
            builder.RegisterComponent(peerClient);

            builder.Register<GroupManager>(Lifetime.Singleton);

            var redisMessagingClient = RedisMessagingClientProvider.Provide(assetHelper.MultiplayConfig.RedisMessagingConfig);
            var queuingMessagingClient = new QueuingMessagingClient(redisMessagingClient);

            builder.Register<MultiplayClient>(Lifetime.Singleton).WithParameter(queuingMessagingClient).WithParameter<INetworkObjectsProvider>(networkObjectsProvider);

            var textChatClient = RedisMessagingClientProvider.Provide(assetHelper.MessagingConfig.RedisMessagingConfig);
            builder.RegisterComponent<MessagingClient>(textChatClient);
            var voiceChatClient = VoiceChatClientProvider.Provide(peerClient, assetHelper.VoiceChatConfig);
            builder.RegisterComponent(voiceChatClient);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
