using Extreal.Core.Logging;
using Extreal.Integration.Web.Common.Video;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Video
{
    public class LandscapeVideoPlayer : LandscapePlayerBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LandscapeVideoPlayer));
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly AppState appState;
        private readonly LandscapeConfig landscapeConfig;
        private readonly EVideoPlayer videoPlayer;
        private readonly string videoFileName;

        private bool isPlaying;

        public LandscapeVideoPlayer(AppState appState, LandscapeConfig landscapeConfig, EVideoPlayer videoPlayer, string videoFileName)
        {
            this.appState = appState;
            this.landscapeConfig = landscapeConfig;

            this.videoPlayer = videoPlayer;
            this.videoFileName = videoFileName;

            this.videoPlayer.SetUrl(AppUtils.ConcatUrl(this.landscapeConfig.BaseUrl, this.videoFileName));

            this.videoPlayer.OnErrorReceived
                .Subscribe(ErrorReceived)
                .AddTo(disposables);

            this.videoPlayer.OnPrepareCompleted
                .Subscribe(_ => PrepareCompleted())
                .AddTo(disposables);
        }

        private void ErrorReceived(string message)
        {
            OnErrorOccurredSubject.OnNext(Unit.Default);
            Logger.LogError(message);
            if (!isPlaying)
            {
                appState.SetLandscapeInitialized(true);
            }
        }

        protected override void ReleaseManagedResources()
        {
            videoPlayer.Stop();
            disposables.Dispose();
            base.ReleaseManagedResources();
        }

        public override void Play()
            => videoPlayer.Prepare();

        private void PrepareCompleted()
        {
            videoPlayer.Play();
            isPlaying = true;
            appState.SetLandscapeInitialized(true);
        }
    }
}
