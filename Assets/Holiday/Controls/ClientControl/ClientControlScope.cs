using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.Integration.Multiplay.Messaging;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Extreal.Integration.Messaging.Redis;
using Extreal.Integration.Messaging;
using Extreal.Integration.Chat.OME;
using Extreal.Integration.SFU.OME;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        [SerializeField] private NetworkObjectsProvider networkObjectsProvider;

        protected override void Configure(IContainerBuilder builder)
        {
            var assetHelper = Parent.Container.Resolve<AssetHelper>();

            builder.Register<GroupManager>(Lifetime.Singleton);

            var redisMessagingClient = RedisMessagingClientProvider.Provide(assetHelper.MultiplayConfig.RedisMessagingConfig);
            var queuingMessagingClient = new QueuingMessagingClient(redisMessagingClient);
#if HOLIDAY_LOAD_CLIENT
            builder.Register<MultiplayClient, MultiplayClientForTest>(Lifetime.Singleton).WithParameter(queuingMessagingClient).WithParameter<INetworkObjectsProvider>(networkObjectsProvider);
#else
            builder.Register<MultiplayClient>(Lifetime.Singleton).WithParameter(queuingMessagingClient).WithParameter<INetworkObjectsProvider>(networkObjectsProvider);
#endif

            var textChatClient = RedisMessagingClientProvider.Provide(assetHelper.MessagingConfig.RedisMessagingConfig);
            builder.RegisterComponent<MessagingClient>(textChatClient);

            var omeClient = OmeClientProvider.Provide(assetHelper.OmeConfig);
            builder.RegisterComponent(omeClient);

            var voiceChatClient = VoiceChatClientProvider.Provide(omeClient, assetHelper.VoiceChatConfig);
            builder.RegisterComponent(voiceChatClient);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
