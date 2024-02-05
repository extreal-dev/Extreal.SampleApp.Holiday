using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Messaging;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlPresenter : StagePresenterBase
    {
        private readonly AssetHelper assetHelper;
        private readonly MessagingClient messagingClient;
        private readonly TextChatControlView textChatControlView;
        private TextChatRoom textChatRoom;

        public TextChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            MessagingClient messagingClient,
            TextChatControlView textChatControlView
        ) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.messagingClient = messagingClient;
            this.textChatControlView = textChatControlView;
        }

        [SuppressMessage("CodeCracker", "CC0020")]
        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            textChatRoom = new TextChatRoom(messagingClient, appState, assetHelper);
            textChatRoom.AddTo(sceneDisposables);

            textChatControlView.OnSendButtonClicked
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .Subscribe(message =>
                {
                    textChatRoom.SendMessageAsync(message).Forget();
                    appState.StageState.CountUpTextChats();

                    textChatControlView.ShowMessage(message);
                })
                .AddTo(sceneDisposables);

            textChatRoom.OnMessageReceived
                .Subscribe(textChatControlView.ShowMessage)
                .AddTo(sceneDisposables);

            appState.OnMessageSent
                .Subscribe(textChatRoom.SendEveryoneMessageAsync)
                .AddTo(sceneDisposables);

            textChatRoom.JoinAsync().Forget();
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            if (AppUtils.IsSpace(stageName))
            {
                return;
            }

            textChatRoom.LeaveAsync().Forget();
        }
    }
}
