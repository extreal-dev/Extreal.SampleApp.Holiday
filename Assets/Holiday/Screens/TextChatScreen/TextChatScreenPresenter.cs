using System;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using UniRx;
using Extreal.SampleApp.Holiday.Models;

namespace Extreal.SampleApp.Holiday.Screens.TextChatScreen
{
    public class TextChatScreenPresenter : IInitializable, IDisposable
    {
        private readonly TextChatScreenView textChatScreenView;
        private readonly TextChatChannel textChatChannel;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public TextChatScreenPresenter
        (
            TextChatScreenView textChatScreenView,
            TextChatChannel textChatChannel
        )
        {
            this.textChatScreenView = textChatScreenView;
            this.textChatChannel = textChatChannel;
        }

        public void Initialize()
        {
            textChatScreenView.OnSendButtonClicked
                .Subscribe(textChatChannel.SendTextMessage)
                .AddTo(disposables);

            textChatChannel.OnTextMessageReceived
                .Subscribe(textChatScreenView.ShowMessage)
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
