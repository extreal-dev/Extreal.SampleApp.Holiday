using System;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using UniRx;
using Extreal.SampleApp.Holiday.Models;

namespace Extreal.SampleApp.Holiday.Screens.VoiceChatScreen
{
    public class VoiceChatScreenPresenter : IInitializable, IDisposable
    {
        private readonly VoiceChatScreenView voiceChatScreenView;
        private readonly VoiceChatChannel voiceChatChannel;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public VoiceChatScreenPresenter
        (
            VoiceChatScreenView voiceChatScreenView,
            VoiceChatChannel voiceChatChannel
        )
        {
            this.voiceChatScreenView = voiceChatScreenView;
            this.voiceChatChannel = voiceChatChannel;
        }

        public void Initialize()
        {
            voiceChatScreenView.OnMuteButtonClicked
                .Subscribe(_ => voiceChatChannel.ToggleMuteAsync().Forget())
                .AddTo(disposables);

            voiceChatChannel.OnMuted
                .Subscribe(voiceChatScreenView.SetMutedString)
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
