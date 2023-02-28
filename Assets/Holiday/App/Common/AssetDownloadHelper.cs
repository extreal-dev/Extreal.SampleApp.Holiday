using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Screens.ConfirmationScreen;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public class AssetDownloadHelper
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AssetDownloadHelper));

        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AssetProvider assetProvider;
        private readonly AppState appState;

        public AssetDownloadHelper(
            StageNavigator<StageName, SceneName> stageNavigator, AssetProvider assetProvider, AppState appState)
        {
            this.stageNavigator = stageNavigator;
            this.assetProvider = assetProvider;
            this.appState = appState;
        }

        public async UniTask DownloadAsync(string assetName, StageName nextStage)
        {
            Action nextAction = () => stageNavigator.ReplaceAsync(nextStage).Forget();
            var size = await assetProvider.GetDownloadSizeAsync(assetName);
            if (size != 0)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Download asset: {assetName} nextStage: {nextStage}");
                }
                var sizeUnit = AppUtils.GetSizeUnit(size);
                appState.SetConfirmation(new Confirmation(
                    $"Download {sizeUnit.Item1:F2}{sizeUnit.Item2} of data.",
                    () => assetProvider.DownloadAsync(assetName, nextAction: nextAction).Forget()));
            }
            else
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"No download asset: {assetName} nextStage: {nextStage}");
                }
                nextAction();
            }
        }
    }
}
