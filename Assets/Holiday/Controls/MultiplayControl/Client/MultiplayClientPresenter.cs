using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.MultiplyControl.Client
{
    public class MultiplayClientPresenter : StagePresenterBase
    {
        private readonly NgoClient ngoClient;
        private readonly AssetHelper assetHelper;
        private MultiplayClient multiplayClient;

        public MultiplayClientPresenter
        (
            NgoClient ngoClient,
            AssetHelper assetHelper,
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
            this.ngoClient = ngoClient;
            this.assetHelper = assetHelper;
        }

        [SuppressMessage("CodeCracker", "CC0092")]
        protected override void Initialize
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables
        )
        {
            multiplayClient = new MultiplayClient(ngoClient, assetHelper, appState);
            sceneDisposables.Add(multiplayClient);

            multiplayClient.IsPlayerSpawned
                .Subscribe(appState.SetMultiplayReady)
                .AddTo(sceneDisposables);

            Observable
                .CombineLatest(appState.SpaceReady, appState.P2PReady)
                .Where(readies => readies.All(ready => ready) && appState.IsClient)
                .Subscribe(_ => multiplayClient.JoinAsync().Forget())
                .AddTo(sceneDisposables);

            appState.PlayingReady
                .Skip(1)
                .Where(ready => ready)
                .Subscribe(_ => multiplayClient.ResetPosition())
                .AddTo(sceneDisposables);
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            appState.SetMultiplayReady(false);
            multiplayClient.LeaveAsync().Forget();
        }
    }
}
