using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App;
using UnityEngine;
using UnityEngine.Networking;
using Extreal.Core.Logging;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Image
{
    public class LandscapeImagePlayer : LandscapePlayerBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LandscapeImagePlayer));
        private readonly AppState appState;
        private readonly LandscapeConfig landscapeConfig;
        private readonly string imageUrl;
        private readonly Renderer panoramicRenderer;

        private bool isPlaying;

        public LandscapeImagePlayer(AppState appState, LandscapeConfig landscapeConfig, Renderer panoramicRenderer, string imageFileName)
        {
            this.appState = appState;
            this.landscapeConfig = landscapeConfig;
            imageUrl = AppUtils.ConcatUrl(this.landscapeConfig.BaseUrl, imageFileName);
            this.panoramicRenderer = panoramicRenderer;

        }

        public override void Play() => DoPlayAsync().Forget();

        private async UniTask<Texture> GetTextureAsync(string imageUrl)
        {
            using var request = UnityWebRequestTexture.GetTexture(imageUrl);
            _ = await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                ErrorReceived(request.error);
            }

            return ((DownloadHandlerTexture)request.downloadHandler).texture;
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

        private async UniTask DoPlayAsync()
        {
            panoramicRenderer.material.mainTexture = await GetTextureAsync(imageUrl);
            appState.SetLandscapeInitialized(true);
        }
    }
}
