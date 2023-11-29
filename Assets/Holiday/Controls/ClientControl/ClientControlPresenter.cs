using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.LiveKit;
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
        private readonly PubSubMultiplayClient liveKitMultiplayClient;

        public ClientControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            PubSubMultiplayClient liveKitMultiplayClient) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.liveKitMultiplayClient = liveKitMultiplayClient;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
            => InitializeLiveKitMultiplayClient(stageNavigator, appState, sceneDisposables);

        private void InitializeLiveKitMultiplayClient(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            liveKitMultiplayClient.OnConnectionApprovalRejected
                .Subscribe(_ =>
                {
                    appState.Notify(assetHelper.MessageConfig.MultiplayConnectionApprovalRejectedMessage);
                    stageNavigator.ReplaceAsync(StageName.GroupSelectionStage).Forget();
                })
                .AddTo(sceneDisposables);

            // liveKitMultiplayClient.OnConnectRetrying
            //     .Subscribe(retryCount => AppUtils.NotifyRetrying(
            //         appState,
            //         assetHelper.MessageConfig.MultiplayConnectRetryMessage,
            //         retryCount))
            //     .AddTo(sceneDisposables);

            // liveKitMultiplayClient.OnConnectRetried
            //     .Subscribe(result => AppUtils.NotifyRetried(
            //         appState,
            //         result,
            //         assetHelper.MessageConfig.MultiplayConnectRetrySuccessMessage,
            //         assetHelper.MessageConfig.MultiplayConnectRetryFailureMessage))
            //     .AddTo(sceneDisposables);

            liveKitMultiplayClient.OnUnexpectedDisconnected
                .Subscribe(_ =>
                    appState.Notify(assetHelper.MessageConfig.MultiplayUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);
        }
    }
}
