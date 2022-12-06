using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.TextChatScreen
{
    public class TextChatScreenScope : LifetimeScope
    {
        [SerializeField] private TextChatScreenView textChatScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(textChatScreenView);

            builder.RegisterEntryPoint<TextChatScreenPresenter>();
        }
    }
}
