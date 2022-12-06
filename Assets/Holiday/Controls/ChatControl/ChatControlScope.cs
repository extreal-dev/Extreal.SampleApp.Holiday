using Extreal.SampleApp.Holiday.Models;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class ChatControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TextChatChannel>().AsSelf();
            builder.RegisterEntryPoint<VoiceChatChannel>().AsSelf();

            builder.RegisterEntryPoint<ChatControlPresenter>();
        }
    }
}
