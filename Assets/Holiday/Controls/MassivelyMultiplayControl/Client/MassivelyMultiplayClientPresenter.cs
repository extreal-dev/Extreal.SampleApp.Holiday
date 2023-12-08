using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.Common;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl.Client
{
    public class MassivelyMultiplayClientPresenter : StagePresenterBase
    {
        private readonly ExtrealMultiplayClient liveKitMultiplayClient;
        private readonly AssetHelper assetHelper;
        private MassivelyMultiplayClient multiplayClient;

        public MassivelyMultiplayClientPresenter
        (
            ExtrealMultiplayClient liveKitMultiplayClient,
            AssetHelper assetHelper,
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
            this.liveKitMultiplayClient = liveKitMultiplayClient;
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
            if (!appState.IsMassivelyForCommunication)
            {
                return;
            }

            multiplayClient = new MassivelyMultiplayClient(liveKitMultiplayClient, assetHelper, appState);
            sceneDisposables.Add(multiplayClient);

            multiplayClient.IsPlayerSpawned
                .Subscribe(appState.SetMultiplayReady)
                .AddTo(sceneDisposables);

            appState.SpaceReady
                .Where(ready => ready && !appState.MultiplayReady.Value)
                .Subscribe(_ => multiplayClient.JoinAsync().Forget())
                .AddTo(sceneDisposables);

            appState.PlayingReady
                .Skip(1)
                .Where(ready => ready)
                .Subscribe(_ => multiplayClient.ResetPosition())
                .AddTo(sceneDisposables);

            appState.OnMessageSent
                .Subscribe(multiplayClient.SendToOthers)
                .AddTo(sceneDisposables);
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            if (!appState.IsMassivelyForCommunication || AppUtils.IsSpace(stageName))
            {
                return;
            }
            appState.SetMultiplayReady(false);
            multiplayClient.LeaveAsync().Forget();
        }
    }
}
