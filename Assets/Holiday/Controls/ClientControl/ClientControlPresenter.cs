using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.NGO.Dev;
using Extreal.SampleApp.Holiday.App.P2P;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlPresenter : StagePresenterBase
    {
        private readonly AssetHelper assetHelper;
        private readonly GroupManager groupManager;
        private readonly VivoxClient vivoxClient;
        private readonly NgoHost ngoHost;
        private readonly NgoClient ngoClient;
        private readonly IConnectionSetter connectionSetter;

        public ClientControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            GroupManager groupManager,
            VivoxClient vivoxClient,
            NgoHost ngoHost,
            NgoClient ngoClient,
            IConnectionSetter connectionSetter) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.groupManager = groupManager;
            this.vivoxClient = vivoxClient;
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
            InitializeVivoxClient(appState, sceneDisposables);
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
                .Where(_ => appState.Role == Role.Client)
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
                .Where(_ => appState.Role == Role.Client)
                .Subscribe(_ =>
                    appState.Notify(assetHelper.MessageConfig.MultiplayUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);
        }

        private void InitializeVivoxClient(AppState appState, CompositeDisposable sceneDisposables)
        {
            vivoxClient.OnConnectRetrying
                .Subscribe(retryCount => AppUtils.NotifyRetrying(
                    appState,
                    assetHelper.MessageConfig.ChatConnectRetryMessage,
                    retryCount))
                .AddTo(sceneDisposables);

            vivoxClient.OnConnectRetried
                .Subscribe(result => AppUtils.NotifyRetried(
                    appState,
                    result,
                    assetHelper.MessageConfig.ChatConnectRetrySuccessMessage,
                    assetHelper.MessageConfig.ChatConnectRetryFailureMessage))
                .AddTo(sceneDisposables);

            vivoxClient.OnRecoveryStateChanged
                .Where(recoveryState => recoveryState == ConnectionRecoveryState.FailedToRecover)
                .Subscribe(_ => appState.Notify(assetHelper.MessageConfig.ChatUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);

            vivoxClient.LoginAsync(new VivoxAuthConfig(nameof(Holiday))).Forget();
        }
    }
}
