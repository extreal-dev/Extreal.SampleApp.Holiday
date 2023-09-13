using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using UnityEngine.Video;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Video
{
    public class LandscapeVideoPlayerFactory : ILandscapePlayerFactory
    {
        public LandscapeType LandscapeType => LandscapeType.Video;

        private readonly AppState appState;
        private readonly VideoPlayer videoPlayer;
        private readonly LandscapeConfig landscapeConfig;

        public LandscapeVideoPlayerFactory(AppState appState, VideoPlayer videoPlayer, AssetHelper assetHelper)
        {
            this.appState = appState;
            this.videoPlayer = videoPlayer;
            landscapeConfig = assetHelper.LandscapeConfig;
        }

#pragma warning disable CS1998
        public async UniTask<ILandscapePlayer> CreateAsync(StageName stageName)
            => new LandscapeVideoPlayer(appState, landscapeConfig, videoPlayer, $"{stageName}.mp4");
#pragma warning restore CS1998
    }
}
