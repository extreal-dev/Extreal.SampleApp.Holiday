using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.P2P.Dev;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using SocketIOClient;
using UniRx;

namespace Extreal.SampleApp.Holiday.Holiday.Controls.P2PControl
{
    public class P2PControlPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(P2PControlPresenter));

        private readonly AssetHelper assetHelper;
        private readonly PeerClient peerClient;

        public P2PControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            PeerClient peerClient) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.peerClient = peerClient;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            peerClient.OnStarted
                .Subscribe(_ => appState.SetP2PReady(true))
                .AddTo(sceneDisposables);

            peerClient.OnHostNameAlreadyExists
                .Subscribe(_ =>
                {
                    appState.Notify(assetHelper.MessageConfig.P2PHostNameAlreadyExistsMessage);
                    stageNavigator.ReplaceAsync(StageName.GroupSelectionStage);
                }).AddTo(sceneDisposables);

            peerClient.OnDisconnected
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    appState.Notify(assetHelper.MessageConfig.P2PUnexpectedDisconnectedMessage);
                }).AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(
            StageName stageName, AppState appState, CompositeDisposable stageDisposables)
        {
            if (peerClient.IsRunning)
            {
                return;
            }
            StartPeerClientAsync(appState).Forget();
        }

        private async UniTask StartPeerClientAsync(AppState appState)
        {
            try
            {
                if (appState.IsHost)
                {
                    await peerClient.StartHostAsync(appState.GroupName);
                }
                else
                {
                    await peerClient.StartClientAsync(appState.GroupId);
                }
            }
            catch (ConnectionException e)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(e.Message);
                }
                appState.Notify(assetHelper.MessageConfig.P2PStartFailureMessage);
            }
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            if (AppUtils.IsSpace(stageName))
            {
                return;
            }
            peerClient.Stop();
            appState.SetP2PReady(false);
        }
    }
}
