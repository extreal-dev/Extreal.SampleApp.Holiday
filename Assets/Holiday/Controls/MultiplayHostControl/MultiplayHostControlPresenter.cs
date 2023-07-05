using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Extreal.NGO;
using Extreal.SampleApp.Holiday.App.P2P;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayHostControl
{
    public class MultiplayHostControlPresenter : StagePresenterBase
    {
        private readonly NgoHost ngoHost;
        private readonly GameObject playerPrefab;
        private readonly AssetHelper assetHelper;

        private MultiplayHost multiplayHost;

        public MultiplayHostControlPresenter
        (
            NgoHost ngoHost,
            GameObject playerPrefab,
            AssetHelper assetHelper,
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
            this.ngoHost = ngoHost;
            this.playerPrefab = playerPrefab;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables
        )
        {
            if (appState.Role != Role.Host)
            {
                return;
            }

            multiplayHost = new MultiplayHost(ngoHost, playerPrefab, assetHelper);
            sceneDisposables.Add(multiplayHost);

            appState.SpaceReady
                .First(ready => ready)
                .Subscribe(_ => multiplayHost.StartHostAsync().Forget())
                .AddTo(sceneDisposables);
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            if (appState.Role != Role.Host)
            {
                return;
            }

            if (AppUtils.IsSpace(stageName))
            {
                return;
            }

            multiplayHost.StopHostAsync().Forget();
        }
    }
}
