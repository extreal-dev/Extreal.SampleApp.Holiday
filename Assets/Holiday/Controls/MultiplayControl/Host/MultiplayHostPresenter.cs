using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl.Host
{
    public class MultiplayHostPresenter : StagePresenterBase
    {
        private readonly AssetHelper assetHelper;
        private readonly NgoServer ngoHost;
        private readonly GameObject playerPrefab;

        private MultiplayHost multiplayHost;

        public MultiplayHostPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            NgoServer ngoHost,
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
            if (!appState.IsHost)
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
            if (!appState.IsHost || AppUtils.IsSpace(stageName))
            {
                return;
            }
            multiplayHost.StopHostAsync().Forget();
        }
    }
}
