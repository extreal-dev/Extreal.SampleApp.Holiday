using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App;
using UnityEngine;
using UnityEngine.Networking;
using Extreal.Core.Logging;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Image
{
    public class LandscapeImagePlayer : LandscapePlayerBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LandscapeImagePlayer));
        private readonly AppState appState;
        private readonly LandscapeConfig landscapeConfig;
        private readonly string imageUrl;
        private readonly Renderer panoramicRenderer;

        public LandscapeImagePlayer(AppState appState, LandscapeConfig landscapeConfig, Renderer panoramicRenderer, string imageFileName)
        {
            this.appState = appState;
            this.landscapeConfig = landscapeConfig;
            imageUrl = AppUtils.ConcatUrl(this.landscapeConfig.BaseUrl, imageFileName);
            this.panoramicRenderer = panoramicRenderer;

        }

        public override void Play() => DoPlayAsync().Forget();

        private static async UniTask<Texture> GetTextureAsync(string imageUrl)
        {
            using var request = UnityWebRequestTexture.GetTexture(imageUrl);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"error occured! {request.error}");
                }
            }
            else
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"sueccess!");
                }
                return ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
            return null;
        }
        // TODO:
        // private void ErrorReceived( string message)

        private async UniTask DoPlayAsync()
        {
            panoramicRenderer.material.mainTexture = await GetTextureAsync(imageUrl);
            appState.SetLandscapeInitialized(true);
        }
    }
}
