using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.NGO.Dev;
using Extreal.SampleApp.Holiday.App.P2P;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl.Host
{
    public class MultiplayHostPresenter : StagePresenterBase
    {
        private readonly AssetHelper assetHelper;
        private readonly NgoHost ngoHost;
        private readonly GameObject playerPrefab;

        private MultiplayHost multiplayHost;

        public MultiplayHostPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            NgoHost ngoHost,
            GameObject playerPrefab
        ) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.ngoHost = ngoHost;
            this.playerPrefab = playerPrefab;
        }

        [SuppressMessage("Cracker", "CC0092")]
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

            Observable
                .CombineLatest(appState.SpaceReady, appState.P2PReady)
                .Where(readies => readies.All(ready => ready))
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