using System;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using UniRx;
using Extreal.SampleApp.Holiday.Models;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlPresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly TextChatControlView textChatScreenView;
        private readonly TextChatChannel textChatChannel;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public TextChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            TextChatControlView textChatScreenView,
            TextChatChannel textChatChannel,
            AppState appState
        )
        {
            this.stageNavigator = stageNavigator;
            this.textChatScreenView = textChatScreenView;
            this.textChatChannel = textChatChannel;
            this.appState = appState;
        }

        public void Initialize()
        {
            stageNavigator.OnStageTransitioned
                .Subscribe(stageName => textChatChannel.SetChannelName($"HolidayTextChat{stageName}"))
                .AddTo(disposables);

            stageNavigator.OnStageTransitioning
                .Subscribe(_ => textChatChannel.Leave())
                .AddTo(disposables);

            textChatChannel.InText
                .Subscribe(appState.SetInText)
                .AddTo(disposables);

            textChatScreenView.OnSendButtonClicked
                .Subscribe(textChatChannel.SendTextMessage)
                .AddTo(disposables);

            textChatChannel.OnTextMessageReceived
                .Subscribe(textChatScreenView.ShowMessage)
                .AddTo(disposables);

            textChatChannel.OnUnexpectedDisconnected
                .Subscribe(_ =>
                {
                    appState.SetErrorMessage("Unexpected disconnection from vivox server has occurred");
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(disposables);

            appState.InMultiplay
                .Where(inMultiplay => inMultiplay)
                .Subscribe(_ => textChatChannel.Join())
                .AddTo(disposables);

            textChatChannel.Login();
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
