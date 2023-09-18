using Extreal.Integration.AssetWorkflow.Addressables;
using Extreal.SampleApp.Holiday.App;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Image
{
    public class LandscapeImagePlayer : LandscapePlayerBase
    {
        private readonly AppState appState;
        private readonly AssetDisposable<Sprite> disposableSprite;
        private readonly Renderer panoramicRenderer;

        public LandscapeImagePlayer(AppState appState, Renderer panoramicRenderer, AssetDisposable<Sprite> disposableSprite)
        {
            this.appState = appState;
            this.disposableSprite = disposableSprite;
            this.panoramicRenderer = panoramicRenderer;
        }

        protected override void ReleaseManagedResources()
            => disposableSprite.Dispose();

        public override void Play()
        {
            panoramicRenderer.material.mainTexture = disposableSprite.Result.texture;
            appState.SetLandscapeInitialized(true);
        }
    }
}
