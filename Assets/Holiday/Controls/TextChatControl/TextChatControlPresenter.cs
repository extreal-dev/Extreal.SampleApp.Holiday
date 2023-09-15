using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.WebRTC;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlPresenter : StagePresenterBase
    {
        private readonly TextChatClient textChatClient;
        private readonly TextChatControlView textChatControlView;

        public TextChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            TextChatClient textChatClient,
            TextChatControlView textChatControlView
        ) : base(stageNavigator, appState)
        {
            this.textChatClient = textChatClient;
            this.textChatControlView = textChatControlView;
        }

        [SuppressMessage("CodeCracker", "CC0020")]
        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
            => textChatControlView.OnSendButtonClicked
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .Subscribe(message =>
                {
                    textChatClient.Send(message);
                    appState.StageState.CountUpTextChats();
                })
                .AddTo(sceneDisposables);

        [SuppressMessage("CodeCracker", "CC0092")]
        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
            textChatClient.OnMessageReceived
                .Subscribe(textChatControlView.ShowMessage)
                .AddTo(stageDisposables);
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
            => textChatClient.Clear();
    }
}
