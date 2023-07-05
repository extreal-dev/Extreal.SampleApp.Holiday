using System.Diagnostics.CodeAnalysis;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlPresenter : StagePresenterBase
    {
        private readonly VivoxClient vivoxClient;
        private readonly TextChatControlView textChatControlView;

        private TextChatChannel textChatChannel;

        public TextChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            VivoxClient vivoxClient,
            TextChatControlView textChatControlView
        ) : base(stageNavigator, appState)
        {
            this.vivoxClient = vivoxClient;
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
                    textChatChannel.SendMessage(message);
                    appState.StageState.CountUpTextChats();
                })
                .AddTo(sceneDisposables);

        protected override void OnStageEntered(
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables)
        {
            textChatChannel = new TextChatChannel(vivoxClient, $"HolidayTextChat{stageName}");
            stageDisposables.Add(textChatChannel);

            textChatChannel.OnConnected
                .Subscribe(appState.SetTextChatReady)
                .AddTo(stageDisposables);

            textChatChannel.OnMessageReceived
                .Subscribe(textChatControlView.ShowMessage)
                .AddTo(stageDisposables);

            textChatChannel.JoinAsync().Forget();
        }

        protected override void OnStageExiting(
            StageName stageName,
            AppState appState)
        {
            appState.SetTextChatReady(false);
            textChatChannel.Leave();
        }
    }
}
