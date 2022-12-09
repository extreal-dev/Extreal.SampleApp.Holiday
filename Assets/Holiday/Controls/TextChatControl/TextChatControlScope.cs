using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlScope : LifetimeScope
    {
        [SerializeField] private TextChatControlView textChatControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(textChatControlView);

            builder.RegisterEntryPoint<TextChatControlPresenter>();
        }
    }
}
