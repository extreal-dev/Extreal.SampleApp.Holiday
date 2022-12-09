using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MultiplayControlPresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly MultiplayRoom multiplayRoom;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public MultiplayControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            MultiplayRoom multiplayRoom,
            AppState appState
        )
        {
            this.stageNavigator = stageNavigator;
            this.multiplayRoom = multiplayRoom;
            this.appState = appState;
        }

        public void Initialize()
        {
            multiplayRoom.Initialize();

            stageNavigator.OnStageTransitioned
                .Subscribe(_ => OnStageEntered())
                .AddTo(disposables);

            stageNavigator.OnStageTransitioning
                .Subscribe(_ => OnStageExiting())
                .AddTo(disposables);

            multiplayRoom.IsPlayerSpawned
                .Subscribe(appState.SetInMultiplay)
                .AddTo(disposables);

            multiplayRoom.OnConnectionApprovalRejected
                .Subscribe(_ =>
                {
                    appState.SetErrorMessage("The space is full");
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(disposables);

            multiplayRoom.OnUnexpectedDisconnected
                .Subscribe(_ =>
                {
                    appState.SetErrorMessage("Unexpected disconnection from multiplay server has occurred");
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(disposables);

            multiplayRoom.OnConnectFailed
                .Subscribe(_ =>
                {
                    appState.SetErrorMessage("Connection to multiplay server is failed");
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage);
                })
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public void OnStageEntered()
            => multiplayRoom.JoinAsync(appState.Avatar.Value.AssetName).Forget();

        public void OnStageExiting()
            => multiplayRoom.LeaveAsync().Forget();
    }
}
