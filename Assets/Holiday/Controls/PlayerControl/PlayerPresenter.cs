namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    using System;
    using Cysharp.Threading.Tasks;
    using Models;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class PlayerPresenter : IInitializable, IStartable, IDisposable
    {
        [Inject] private PlayerView playerView;
        [Inject] private Player player;

        private readonly CompositeDisposable compositeDisposable = new();

        public void Initialize()
        {
            playerView.UnfollowPlayer();

            player.IsPlaying.Subscribe(value =>
            {
                if (value)
                {
                    playerView.FollowPlayer(player.CameraRoot);
                }
                else
                {
                    playerView.UnfollowPlayer();
                }
            }).AddTo(compositeDisposable);
        }

        public void Start() => player.CreateAsync().Forget();

        public void Dispose()
        {
            compositeDisposable?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
