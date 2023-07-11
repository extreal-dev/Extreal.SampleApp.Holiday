using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.P2P.Dev;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.P2P;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Holiday.Controls.P2PControl
{
    public class P2PControlPresenter : StagePresenterBase
    {
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
            peerClient.OnStarted.Subscribe(_ => appState.SetP2PReady(true)).AddTo(sceneDisposables);

            peerClient.OnHostNameAlreadyExists.Subscribe(_ =>
            {
                appState.Notify(assetHelper.MessageConfig.P2PHostNameAlreadyExistsMessage);
                stageNavigator.ReplaceAsync(StageName.GroupSelectionStage);
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
            if (appState.Role == Role.Host)
            {
                await peerClient.StartHostAsync(appState.GroupName);
            }
            else
            {
                await peerClient.StartClientAsync(appState.GroupId);
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
