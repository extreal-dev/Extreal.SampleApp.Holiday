using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.P2PControl
{
    public class P2PControlPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(P2PControlPresenter));

        private readonly AssetHelper assetHelper;
        private readonly PeerClient peerClient;

        private Action handleOnHostNameAlreadyExists;

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

            peerClient.OnConnectFailed
                .Subscribe(_ => appState.Notify(assetHelper.MessageConfig.P2PStartFailureMessage))
                .AddTo(sceneDisposables);

            peerClient.OnDisconnected
                .Subscribe(_ => appState.Notify(assetHelper.MessageConfig.P2PUnexpectedDisconnectedMessage))
                .AddTo(sceneDisposables);

            handleOnHostNameAlreadyExists = () =>
            {
                appState.Notify(assetHelper.MessageConfig.P2PHostNameAlreadyExistsMessage);
                stageNavigator.ReplaceAsync(StageName.GroupSelectionStage).Forget();
            };
        }

        protected override void OnStageEntered(
            StageName stageName, AppState appState, CompositeDisposable stageDisposables) => StartPeerClientAsync(appState).Forget();

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
            catch (HostNameAlreadyExistsException e)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(e.Message);
                }
                handleOnHostNameAlreadyExists.Invoke();
            }
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            peerClient.Stop();
            appState.SetP2PReady(false);
        }
    }
}
