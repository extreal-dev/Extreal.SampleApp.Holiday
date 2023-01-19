﻿using Cysharp.Threading.Tasks;
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
        private readonly NgoConfig ngoConfig;
        private readonly AppState appState;
        private MultiplayRoom multiplayRoom;

        public MultiplayControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            NgoClient ngoClient,
            NgoConfig ngoConfig,
            AppState appState) : base(stageNavigator)
        {
            this.stageNavigator = stageNavigator;
            this.ngoClient = ngoClient;
            this.ngoConfig = ngoConfig;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            multiplayRoom = new MultiplayRoom(ngoClient, ngoConfig);
            stageDisposables.Add(multiplayRoom);

            multiplayRoom.IsPlayerSpawned
                .Subscribe(appState.SetInMultiplay)
                .AddTo(stageDisposables);

            multiplayRoom.OnConnectionApprovalRejected
                .Subscribe(_ =>
                {
                    appState.SetNotification("The space is full");
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(stageDisposables);

            multiplayRoom.OnUnexpectedDisconnected
                .Subscribe(_ =>
                    appState.SetNotification("Unexpected disconnection from multiplay server has occurred"))
                .AddTo(stageDisposables);

            multiplayRoom.OnConnectFailed
                .Subscribe(_ => appState.SetNotification("Connection to multiplay server is failed"))
                .AddTo(stageDisposables);

            multiplayRoom.JoinAsync(appState.Avatar.Value.AssetName).Forget();
        }

        protected override void OnStageExiting(StageName stageName)
        {
            appState.SetInMultiplay(false);
            multiplayRoom.LeaveAsync().Forget();
        }
    }
}
