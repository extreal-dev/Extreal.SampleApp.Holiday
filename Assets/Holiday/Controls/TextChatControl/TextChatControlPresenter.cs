using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlPresenter : StagePresenterBase
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly VivoxClient vivoxClient;
        private readonly TextChatControlView textChatControlView;
        private readonly AppState appState;

        private TextChatChannel textChatChannel;

        public TextChatControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            VivoxClient vivoxClient,
            TextChatControlView textChatControlView,
            AppState appState) : base(stageNavigator)
        {
            this.stageNavigator = stageNavigator;
            this.vivoxClient = vivoxClient;
            this.textChatControlView = textChatControlView;
            this.appState = appState;
        }

        [SuppressMessage("CodeCracker", "CC0020")]
        protected override void Initialize(StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            textChatControlView.OnSendButtonClicked
                .Subscribe(message => textChatChannel.SendMessage(message)).AddTo(sceneDisposables);

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            textChatChannel = new TextChatChannel(vivoxClient, $"HolidayTextChat{stageName}");

            textChatChannel.OnConnected
                .Subscribe(appState.SetInText)
                .AddTo(stageDisposables);

            textChatChannel.OnMessageReceived
                .Subscribe(textChatControlView.ShowMessage)
                .AddTo(stageDisposables);

            textChatChannel.OnUnexpectedDisconnected
                .Subscribe(_ =>
                {
                    appState.SetNotification("Unexpected disconnection from vivox server has occurred");
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(stageDisposables);

            textChatChannel.JoinAsync().Forget();
        }

        protected override void OnStageExiting(StageName stageName) => textChatChannel.Leave();
    }
}
