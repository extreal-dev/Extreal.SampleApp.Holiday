using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayControlPresenter : StagePresenterBase
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly NgoClient ngoClient;
        private readonly AppState appState;
        private readonly AssetHelper assetHelper;
        private MultiplayRoom multiplayRoom;

        public MultiplayControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            NgoClient ngoClient,
            AppState appState,
            AssetHelper assetHelper
        ) : base(stageNavigator)
        {
            this.stageNavigator = stageNavigator;
            this.ngoClient = ngoClient;
            this.appState = appState;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            multiplayRoom = new MultiplayRoom(
                ngoClient, assetHelper.NgoConfig, assetHelper, appState.Avatar.Value.AssetName);
            stageDisposables.Add(multiplayRoom);

            multiplayRoom.IsPlayerSpawned
                .Subscribe(appState.SetMultiplayReady)
                .AddTo(stageDisposables);

            multiplayRoom.OnConnectionApprovalRejected
                .Subscribe(_ =>
                {
                    appState.Notify(assetHelper.MessageConfig.MultiplayConnectionApprovalRejectedErrorMessage);
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(stageDisposables);

            multiplayRoom.OnUnexpectedDisconnected
                .Subscribe(_ =>
                    appState.Notify(assetHelper.MessageConfig.MultiplayUnexpectedDisconnectedErrorMessage))
                .AddTo(stageDisposables);

            multiplayRoom.OnConnectFailed
                .Subscribe(_ => appState.Notify(assetHelper.MessageConfig.MultiplayConnectFailedErrorMessage))
                .AddTo(stageDisposables);

            appState.SpaceReady
                .Subscribe(ready =>
                {
                    if (ready)
                    {
                        multiplayRoom.JoinAsync().Forget();
                    }
                })
                .AddTo(stageDisposables);
        }

        protected override void OnStageExiting(StageName stageName)
        {
            appState.SetMultiplayReady(false);
            multiplayRoom.LeaveAsync().Forget();
        }
    }
}
