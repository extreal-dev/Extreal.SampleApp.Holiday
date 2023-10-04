using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine.Video;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Video
{
    public class LandscapeVideoPlayer : LandscapePlayerBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LandscapeVideoPlayer));

        private readonly AppState appState;
        private readonly LandscapeConfig landscapeConfig;
        private readonly VideoPlayer videoPlayer;
        private readonly string videoFileName;

        private bool isPlaying;

        public LandscapeVideoPlayer(AppState appState, LandscapeConfig landscapeConfig, VideoPlayer videoPlayer, string videoFileName)
        {
            this.appState = appState;
            this.landscapeConfig = landscapeConfig;

            this.videoPlayer = videoPlayer;
            this.videoFileName = videoFileName;

            this.videoPlayer.gameObject.SetActive(true);

            this.videoPlayer.url = AppUtils.ConcatUrl(this.landscapeConfig.BaseUrl, this.videoFileName);

            this.videoPlayer.errorReceived += ErrorReceived;
            this.videoPlayer.prepareCompleted += PrepareCompleted;
        }

        private void ErrorReceived(VideoPlayer source, string message)
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
            videoPlayer.errorReceived -= ErrorReceived;
            videoPlayer.prepareCompleted -= PrepareCompleted;
            videoPlayer.gameObject.SetActive(false);
            base.ReleaseManagedResources();
        }

        public override void Play()
            => videoPlayer.Prepare();

        private void PrepareCompleted(VideoPlayer source)
        {
            videoPlayer.Play();
            isPlaying = true;
            appState.SetLandscapeInitialized(true);
        }
    }
}
