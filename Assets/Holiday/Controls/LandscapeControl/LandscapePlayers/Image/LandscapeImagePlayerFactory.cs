using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Image
{
    public class LandscapeImagePlayerFactory : ILandscapePlayerFactory
    {
        public LandscapeType LandscapeType => LandscapeType.Image;

        private readonly AppState appState;
        private readonly AssetHelper assetHelper;
        private readonly Renderer panoramicRenderer;

        public LandscapeImagePlayerFactory(AppState appState, AssetHelper assetHelper, Renderer panoramicRenderer)
        {
            this.appState = appState;
            this.assetHelper = assetHelper;
            this.panoramicRenderer = panoramicRenderer;
        }

        public async UniTask<ILandscapePlayer> CreateAsync(StageName stageName)
        {
            var disposableAsset = await assetHelper.LoadAssetAsync<Sprite>(stageName.ToString());
            return new LandscapeImagePlayer(appState, panoramicRenderer, disposableAsset);
        }
    }
}
