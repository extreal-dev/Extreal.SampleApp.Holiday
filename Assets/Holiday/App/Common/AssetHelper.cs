using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Screens.ConfirmationScreen;
using UnityEngine.AddressableAssets;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public class AssetHelper : DisposableBase
    {
        public AppConfig AppConfig { get; private set; }
        public VivoxAppConfig VivoxAppConfig { get; private set; }
        public NgoConfig NgoConfig { get; private set; }
        public AvatarService AvatarService { get; private set; }

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AssetHelper));

        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AssetProvider assetProvider;
        private readonly AppState appState;

        public AssetHelper(
            StageNavigator<StageName, SceneName> stageNavigator, AssetProvider assetProvider, AppState appState)
        {
            this.stageNavigator = stageNavigator;
            this.assetProvider = assetProvider;
            this.appState = appState;
        }

        public void DownloadCommonAssetAsync(StageName nextStage)
        {
            Func<UniTask> nextFunc = async () =>
            {
                AppConfig = await LoadAsync<AppConfigRepository, AppConfig>(asset => asset.ToAppConfig());
                VivoxAppConfig = await LoadAsync<ChatConfig, VivoxAppConfig>(asset => asset.ToVivoxAppConfig());
                NgoConfig = await LoadAsync<MultiplayConfig, NgoConfig>(asset => asset.ToNgoConfig());
                AvatarService = await LoadAsync<AvatarRepository, AvatarService>(asset => asset.ToAvatarService());
                stageNavigator.ReplaceAsync(nextStage).Forget();
            };
            DownloadAsync(nameof(AppConfigRepository), nextFunc).Forget();
        }

        [SuppressMessage("Design", "CC0031")]
        private async UniTask<TResult> LoadAsync<TAsset, TResult>(Func<TAsset, TResult> toFunc)
        {
            var asset = await assetProvider.LoadAssetAsync<TAsset>(typeof(TAsset).Name);
            var result = toFunc(asset);
            Addressables.Release(asset);
            return result;
        }

        private async UniTask DownloadAsync(string assetName, Func<UniTask> nextFunc)
        {
            var size = await assetProvider.GetDownloadSizeAsync(assetName);
            if (size != 0)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Download asset: {assetName}");
                }
                var sizeUnit = AppUtils.GetSizeUnit(size);
                appState.SetConfirmation(new Confirmation(
                    $"Download {sizeUnit.Item1:F2}{sizeUnit.Item2} of data.",
                    () => assetProvider.DownloadAsync(assetName, nextFunc: nextFunc).Forget()));
            }
            else
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"No download asset: {assetName}");
                }
            }
        }

        public UniTask<T> LoadAsync<T>(string assetName) => assetProvider.LoadAssetAsync<T>(assetName);
    }
}
