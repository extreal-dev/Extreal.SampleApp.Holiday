using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Messaging.Common;
using Extreal.Integration.Multiplay.Common;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MassivelyMultiplyControl.Client
{
    public class MassivelyMultiplayClientPresenter : StagePresenterBase
    {
        private readonly MultiplayClient multiplayClient;
        private readonly QueuingMessagingClient messagingClient;
        private readonly GameObject playerPrefab;
        private readonly AssetHelper assetHelper;
        private MassivelyMultiplayClient massivelyMultiplayClient;

        public MassivelyMultiplayClientPresenter
        (
            MultiplayClient multiplayClient,
            QueuingMessagingClient messagingClient,
            GameObject playerPrefab,
            AssetHelper assetHelper,
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
            this.multiplayClient = multiplayClient;
            this.messagingClient = messagingClient;
            this.playerPrefab = playerPrefab;
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

            massivelyMultiplayClient = new MassivelyMultiplayClient(multiplayClient, messagingClient, playerPrefab, assetHelper, appState);
            sceneDisposables.Add(massivelyMultiplayClient);

            massivelyMultiplayClient.IsPlayerSpawned
                .Subscribe(appState.SetMultiplayReady)
                .AddTo(sceneDisposables);

            appState.SpaceReady
                .Where(ready => ready && !appState.MultiplayReady.Value)
                .Subscribe(_ => massivelyMultiplayClient.JoinAsync().Forget())
                .AddTo(sceneDisposables);

            appState.PlayingReady
                .Skip(1)
                .Where(ready => ready)
                .Subscribe(_ => massivelyMultiplayClient.ResetPosition())
                .AddTo(sceneDisposables);

            appState.OnMessageSent
                .Subscribe(massivelyMultiplayClient.SendToOthers)
                .AddTo(sceneDisposables);
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            if (!appState.IsMassivelyForCommunication || AppUtils.IsSpace(stageName))
            {
                return;
            }
            appState.SetMultiplayReady(false);
            massivelyMultiplayClient.LeaveAsync().Forget();
        }
    }
}
