using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayControlPresenter : StagePresenterBase
    {
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

            appState.SpaceReady
                .Where(ready => ready)
                .Subscribe(_ => multiplayRoom.JoinAsync().Forget())
                .AddTo(stageDisposables);
        }

        protected override void OnStageExiting(StageName stageName)
        {
            appState.SetMultiplayReady(false);
            multiplayRoom.LeaveAsync().Forget();
        }
    }
}
