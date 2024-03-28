using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.Messaging;
using Extreal.Integration.SFU.OME;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlPresenter : StagePresenterBase
    {
        private readonly AssetHelper assetHelper;
        private readonly MultiplayClient multiplayClient;
        private readonly OmeClient omeClient;

        public ClientControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            MultiplayClient multiplayClient,
            OmeClient omeClient) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.multiplayClient = multiplayClient;
            this.omeClient = omeClient;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            InitializeMultiplayClient(stageNavigator, appState, sceneDisposables);
            InitializeOmeClient(appState, sceneDisposables);
        }

        private void InitializeMultiplayClient(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            multiplayClient.OnJoiningApprovalRejected
                .Subscribe(_ =>
                {
                    appState.Notify(assetHelper.MessageConfig.MultiplayConnectionApprovalRejectedMessage);
                    stageNavigator.ReplaceAsync(StageName.GroupSelectionStage).Forget();
                })
                .AddTo(sceneDisposables);

            multiplayClient.OnUnexpectedLeft
                .Subscribe(_ =>
                    appState.Notify(assetHelper.MessageConfig.MultiplayUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);
        }

        private void InitializeOmeClient
        (
            AppState appState,
            CompositeDisposable sceneDisposables
        )
        {
            omeClient.OnUnexpectedLeft
                .Subscribe(reason => appState.Notify(string.Format(assetHelper.MessageConfig.OmeUnexpectedDisconnectedMessage, reason)))
                .AddTo(sceneDisposables);

            omeClient.OnUnexpectedSubscribeFailed
                .Subscribe(reason => appState.Notify(string.Format(assetHelper.MessageConfig.OmeUnexpectedSubscribeFailedMessage, reason)))
                .AddTo(sceneDisposables);

            omeClient.OnJoinRetrying
                .Subscribe(retryCount => AppUtils.NotifyRetrying(
                    appState,
                    assetHelper.MessageConfig.OmeJoinRetryMessage,
                    retryCount))
                .AddTo(sceneDisposables);

            omeClient.OnJoinRetried
                .Subscribe(result => AppUtils.NotifyRetried(
                    appState,
                    result,
                    assetHelper.MessageConfig.OmeJoinRetrySuccessMessage,
                    assetHelper.MessageConfig.OmeJoinRetryFailureMessage))
                .AddTo(sceneDisposables);
        }
    }
}
