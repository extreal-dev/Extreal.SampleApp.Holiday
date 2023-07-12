using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.NGO;
using Extreal.NGO.Dev;
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
        private readonly GroupManager groupManager;
        private readonly NgoHost ngoHost;
        private readonly NgoClient ngoClient;
        private readonly IConnectionSetter connectionSetter;

        public ClientControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            GroupManager groupManager,
            NgoHost ngoHost,
            NgoClient ngoClient,
            IConnectionSetter connectionSetter) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.groupManager = groupManager;
            this.ngoHost = ngoHost;
            this.ngoClient = ngoClient;
            this.connectionSetter = connectionSetter;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            InitializeGroupManager(appState, sceneDisposables);
            InitializeNgoHost();
            InitializeNgoClient(stageNavigator, appState, sceneDisposables);
        }

        private void InitializeGroupManager(
            AppState appState,
            CompositeDisposable sceneDisposables)
            => groupManager.OnGroupsUpdateFailed
                .Subscribe(_ => appState.Notify(assetHelper.MessageConfig.GroupMatchingUpdateFailureMessage))
                .AddTo(sceneDisposables);

        private void InitializeNgoHost()
            => ngoHost.AddConnectionSetter(connectionSetter);

        private void InitializeNgoClient(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            ngoClient.AddConnectionSetter(connectionSetter);

            // FIXME:
            // Hostの場合、Clientが抜けるとOnConnectionApprovalRejectedが発生します。
            ngoClient.OnConnectionApprovalRejected
                .Where(_ => appState.IsClient)
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
                .Where(_ => appState.IsClient)
                .Subscribe(_ =>
                    appState.Notify(assetHelper.MessageConfig.MultiplayUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);
        }
    }
}
