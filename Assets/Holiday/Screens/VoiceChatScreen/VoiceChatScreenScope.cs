using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.VoiceChatScreen
{
    public class VoiceChatScreenScope : LifetimeScope
    {
        [SerializeField] private VoiceChatScreenView voiceChatScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(voiceChatScreenView);

            builder.RegisterEntryPoint<VoiceChatScreenPresenter>();
        }
    }
}
