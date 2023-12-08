using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.Common;
using Extreal.Integration.Multiplay.NGO;
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
        private readonly NgoClient ngoClient;
        private readonly ExtrealMultiplayClient extrealMultiplayClient;

        public ClientControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            NgoClient ngoClient,
            ExtrealMultiplayClient extrealMultiplayClient) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.ngoClient = ngoClient;
            this.extrealMultiplayClient = extrealMultiplayClient;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            InitializeNgoClient(stageNavigator, appState, sceneDisposables);
            InitializeExtrealMultiplayClient(stageNavigator, appState, sceneDisposables);
        }

        private void InitializeNgoClient(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            ngoClient.OnConnectionApprovalRejected
                .Subscribe(_ =>
                {
                    appState.Notify(assetHelper.MessageConfig.MultiplayConnectionApprovalRejectedMessage);
                    stageNavigator.ReplaceAsync(StageName.GroupSelectionStage).Forget();
                })
                .AddTo(sceneDisposables);

            ngoClient.OnConnectRetrying
                .Subscribe(retryCount => AppUtils.NotifyRetrying(
                    appState,
                    assetHelper.MessageConfig.MultiplayConnectRetryMessage,
                    retryCount))
                .AddTo(sceneDisposables);

            ngoClient.OnConnectRetried
                .Subscribe(result => AppUtils.NotifyRetried(
                    appState,
                    result,
                    assetHelper.MessageConfig.MultiplayConnectRetrySuccessMessage,
                    assetHelper.MessageConfig.MultiplayConnectRetryFailureMessage))
                .AddTo(sceneDisposables);

            ngoClient.OnUnexpectedDisconnected
                .Subscribe(_ =>
                    appState.Notify(assetHelper.MessageConfig.MultiplayUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);
        }

        private void InitializeExtrealMultiplayClient(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            extrealMultiplayClient.OnConnectionApprovalRejected
                .Subscribe(_ =>
                {
                    appState.Notify(assetHelper.MessageConfig.MultiplayConnectionApprovalRejectedMessage);
                    stageNavigator.ReplaceAsync(StageName.GroupSelectionStage).Forget();
                })
                .AddTo(sceneDisposables);

            extrealMultiplayClient.OnUnexpectedDisconnected
                .Subscribe(_ =>
                    appState.Notify(assetHelper.MessageConfig.MultiplayUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);
        }
    }
}
