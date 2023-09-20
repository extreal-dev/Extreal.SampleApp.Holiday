using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App;
using UnityEngine;
using UnityEngine.Networking;
using Extreal.Core.Logging;
using UniRx;
using System;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Image
{
    public class LandscapeImagePlayer : LandscapePlayerBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LandscapeImagePlayer));

        private readonly AppState appState;
        private readonly LandscapeConfig landscapeConfig;
        private readonly Renderer panoramicRenderer;
        private readonly string imageUrl;

        public LandscapeImagePlayer(AppState appState, LandscapeConfig landscapeConfig, Renderer panoramicRenderer, string imageFileName)
        {
            this.appState = appState;
            this.landscapeConfig = landscapeConfig;
            this.panoramicRenderer = panoramicRenderer;
            imageUrl = AppUtils.ConcatUrl(this.landscapeConfig.BaseUrl, imageFileName);
        }

        public override void Play() => DoPlayAsync().Forget();

        private async UniTask DoPlayAsync()
        {
            panoramicRenderer.material.mainTexture = await GetTextureAsync(imageUrl);
            appState.SetLandscapeInitialized(true);
        }

        private async UniTask<Texture> GetTextureAsync(string imageUrl)
        {
            try
            {
                using var request = UnityWebRequestTexture.GetTexture(imageUrl);
                _ = await request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    ErrorReceived(request.error);
                }

                return ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
            catch (Exception e)
            {
                ErrorReceived(e.Message);
            }

            return null;
        }

        private void ErrorReceived(string message)
        {
            OnErrorOccurredSubject.OnNext(Unit.Default);
            Logger.LogError(message);
        }
    }
}
