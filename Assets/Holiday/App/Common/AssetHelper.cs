using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Screens.ConfirmationScreen;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public class AssetHelper
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
                AppConfig = await LoadWithAutoReleaseAsync<AppConfigRepository, AppConfig>(asset => asset.ToAppConfig());
                VivoxAppConfig = await LoadWithAutoReleaseAsync<ChatConfig, VivoxAppConfig>(asset => asset.ToVivoxAppConfig());
                NgoConfig = await LoadWithAutoReleaseAsync<MultiplayConfig, NgoConfig>(asset => asset.ToNgoConfig());
                AvatarService = await LoadWithAutoReleaseAsync<AvatarRepository, AvatarService>(asset => asset.ToAvatarService());
                stageNavigator.ReplaceAsync(nextStage).Forget();
            };
            DownloadAsync(nameof(AppConfigRepository), nextFunc).Forget();
        }

        public void DownloadSpaceAssetAsync(string spaceName, StageName nextStage)
        {
#pragma warning disable CS1998
            Func<UniTask> nextFunc = async () =>
            {
                appState.SetSpaceName(spaceName);
                stageNavigator.ReplaceAsync(nextStage).Forget();
            };
            DownloadAsync(spaceName, nextFunc).Forget();
#pragma warning restore CS1998
        }

        [SuppressMessage("Design", "CC0031")]
        private async UniTask<TResult> LoadWithAutoReleaseAsync<TAsset, TResult>(
            Func<TAsset, TResult> toFunc)
        {
            var disposable = await assetProvider.LoadAssetAsync<TAsset>(typeof(TAsset).Name);
            var result = toFunc(disposable.Result);
            disposable.Dispose();
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
                appState.Confirm(new Confirmation(
                    $"Download {sizeUnit.Item1:F2}{sizeUnit.Item2} of data.",
                    () => DownloadOrNotifyErrorAsync(assetName, nextFunc).Forget()));
            }
            else
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"No download asset: {assetName}");
                }
                nextFunc?.Invoke();
            }
        }

        private async UniTask DownloadOrNotifyErrorAsync(string assetName, Func<UniTask> nextFunc)
        {
            try
            {
                await assetProvider.DownloadAsync(assetName, nextFunc: nextFunc);
            }
            catch (Exception e)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug("Exception occurred when downloading assets!", e);
                }
                // Asset download error, so the message is hard coded.
                appState.Notify("Download has failed.");
            }
        }

        public UniTask<AssetDisposable<T>> LoadAssetAsync<T>(string assetName)
            => assetProvider.LoadAssetAsync<T>(assetName);

        public UniTask<AssetDisposable<SceneInstance>> LoadSceneAsync(string assetName)
            => assetProvider.LoadSceneAsync(assetName);
    }
}
