using System;
using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.MultiplayClient.Models;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Controls.SpaceControl
{
    public class SpaceControlPresenter : IInitializable, IDisposable
    {
        private readonly Space space;
        private readonly SpaceControlView spaceControlView;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public SpaceControlPresenter(Space space, SpaceControlView spaceControlView, AppState appState)
        {
            this.space = space;
            this.spaceControlView = spaceControlView;
            this.appState = appState;
        }

        public void Initialize()
        {
            spaceControlView.OnBackButtonClicked
                .Subscribe(_ => space.LeaveAsync().Forget())
                .AddTo(disposables);

            space.OnConnected
                .Subscribe(_ => space.SendPlayerSpawn(appState.Avatar.Value.AssetName))
                .AddTo(disposables);

            space.IsPlayerSpawned
                .Subscribe(appState.SetIsPlaying)
                .AddTo(disposables);

            space.JoinAsync().Forget();
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
