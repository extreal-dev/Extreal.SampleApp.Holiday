using Extreal.Integration.Chat.WebRTC;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.Integration.Multiplay.LiveKit;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        [SerializeField] private LiveKitMultiplayClient liveKitMultiplayClient;

        protected override void Configure(IContainerBuilder builder)
        {
            var assetHelper = Parent.Container.Resolve<AssetHelper>();

            var peerClient = PeerClientProvider.Provide(assetHelper.PeerConfig);
            builder.RegisterComponent(peerClient);

            builder.Register<GroupManager>(Lifetime.Singleton);

            builder.RegisterComponent(liveKitMultiplayClient);

            var textChatClient = TextChatClientProvider.Provide(peerClient);
            builder.RegisterComponent(textChatClient);
            var voiceChatClient = VoiceChatClientProvider.Provide(peerClient, assetHelper.VoiceChatConfig);
            builder.RegisterComponent(voiceChatClient);

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
