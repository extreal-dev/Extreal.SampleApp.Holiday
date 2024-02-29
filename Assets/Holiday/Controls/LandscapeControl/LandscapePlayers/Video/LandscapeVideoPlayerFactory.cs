using Extreal.Integration.Web.Common.Video;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Video
{
    public class LandscapeVideoPlayerFactory : ILandscapePlayerFactory
    {
        public LandscapeType LandscapeType => LandscapeType.Video;

        private readonly AppState appState;
        private readonly EVideoPlayer videoPlayer;
        private readonly LandscapeConfig landscapeConfig;

        public LandscapeVideoPlayerFactory(AppState appState, EVideoPlayer videoPlayer, AssetHelper assetHelper)
        {
            this.appState = appState;
            this.videoPlayer = videoPlayer;
            landscapeConfig = assetHelper.LandscapeConfig;
        }

        public ILandscapePlayer Create(StageName stageName)
            => new LandscapeVideoPlayer(appState, landscapeConfig, videoPlayer, $"{stageName}.mp4");
    }
}
