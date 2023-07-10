using System.Diagnostics.CodeAnalysis;
using Extreal.Chat.Dev;
using Extreal.Core.StageNavigation;
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

        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
            textChatClient.OnMessageReceived
                .Subscribe(textChatControlView.ShowMessage)
                .AddTo(stageDisposables);

            appState.SetTextChatReady(true);
        }

        protected override void OnStageExiting(
            StageName stageName,
            AppState appState)
        {
            appState.SetTextChatReady(false);
            textChatClient.Clear();
        }
    }
}
