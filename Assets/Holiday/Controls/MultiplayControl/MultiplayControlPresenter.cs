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
        private readonly AssetProvider assetProvider;
        private MultiplayRoom multiplayRoom;

        public MultiplayControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            NgoClient ngoClient,
            AppState appState,
            AssetProvider assetProvider
        ) : base(stageNavigator)
        {
            this.stageNavigator = stageNavigator;
            this.ngoClient = ngoClient;
            this.appState = appState;
            this.assetProvider = assetProvider;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
        }

        protected override async void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
            var multiplayConfig = await assetProvider.LoadAssetAsync<MultiplayConfig>(nameof(MultiplayConfig));
            multiplayRoom = new MultiplayRoom(ngoClient, multiplayConfig.ToNgoConfig(), assetProvider);
            stageDisposables.Add(multiplayRoom);

            multiplayRoom.IsPlayerSpawned
                .Subscribe(appState.SetInMultiplay)
                .AddTo(stageDisposables);

            var appConfig = (await assetProvider.LoadAssetAsync<AppConfigRepository>(nameof(AppConfigRepository))).ToAppConfig();

            multiplayRoom.OnConnectionApprovalRejected
                .Subscribe(_ =>
                {
                    appState.SetNotification(appConfig.MultiplayConnectionApprovalRejectedErrorMessage);
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(stageDisposables);

            multiplayRoom.OnUnexpectedDisconnected
                .Subscribe(_ =>
                    appState.SetNotification(appConfig.MultiplayUnexpectedDisconnectedErrorMessage))
                .AddTo(stageDisposables);

            multiplayRoom.OnConnectFailed
                .Subscribe(_ => appState.SetNotification(appConfig.MultiplayConnectFailedErrorMessage))
                .AddTo(stageDisposables);

            appState.SpaceIsReady
                .Subscribe(_ => multiplayRoom.JoinAsync(appState.Avatar.Value.AssetName).Forget())
                .AddTo(stageDisposables);
        }

        protected override void OnStageExiting(StageName stageName)
        {
            appState.SetInMultiplay(false);
            multiplayRoom.LeaveAsync().Forget();
        }
    }
}
