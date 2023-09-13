using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.Retry;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.AssetWorkflow.Addressables;
using Extreal.Integration.Chat.WebRTC;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Screens.ConfirmationScreen;
using UniRx;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Extreal.SampleApp.Holiday.App.AssetWorkflow
{
    public class AssetHelper : DisposableBase
    {
        public IObservable<string> OnDownloading => assetProvider.OnDownloading;
        public IObservable<AssetDownloadStatus> OnDownloaded => assetProvider.OnDownloaded;
        public IObservable<int> OnConnectRetrying => assetProvider.OnConnectRetrying;
        public IObservable<bool> OnConnectRetried => assetProvider.OnConnectRetried;

        public MessageConfig MessageConfig { get; private set; }
        public PeerConfig PeerConfig { get; private set; }
        public HostConfig NgoHostConfig { get; private set; }
        public ClientConfig NgoClientConfig { get; private set; }
        public AvatarConfig AvatarConfig { get; private set; }

        public VoiceChatConfig VoiceChatConfig { get; private set; }

        public SpaceConfig SpaceConfig { get; private set; }
        public LandscapeConfig LandscapeConfig { get; private set; }

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AssetHelper));

        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AssetProvider assetProvider;
        private readonly AppState appState;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable assetDisposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0022")]
        public AssetHelper(
            AppConfig appConfig, StageNavigator<StageName, SceneName> stageNavigator, AppState appState)
        {
            this.stageNavigator = stageNavigator;
            this.appState = appState;
            assetProvider = new AssetProvider(new CountingRetryStrategy(appConfig.DownloadMaxRetryCount));
        }

        public void DownloadCommonAssetAsync(StageName nextStage)
        {
            Func<UniTask> nextFunc = async () =>
            {
                assetDisposables.Clear();
                MessageConfig = await LoadAndAddToDisposablesAsync<MessageConfig>();
                PeerConfig = await LoadAndReleaseAsync<P2PConfig, PeerConfig>(asset => asset.PeerConfig);
                AvatarConfig = await LoadAndAddToDisposablesAsync<AvatarConfig>();
                (NgoHostConfig, NgoClientConfig)
                    = await LoadAndReleaseAsync<MultiplayConfig, (HostConfig, ClientConfig)>(
                        asset => (asset.HostConfig, asset.ClientConfig));
                VoiceChatConfig = await LoadAndReleaseAsync<ChatConfig, VoiceChatConfig>(
                    asset => asset.VoiceChatConfig);
                stageNavigator.ReplaceAsync(nextStage).Forget();
            };
            DownloadAsync(nameof(MessageConfig), nextFunc).Forget();
        }

        protected override void ReleaseManagedResources()
        {
            assetDisposables.Dispose();
            assetProvider.Dispose();
        }

        public void DownloadSpaceAsset(string spaceName, StageName nextStage)
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
        private async UniTask<TResult> LoadAndReleaseAsync<TAsset, TResult>(
            Func<TAsset, TResult> toFunc)
        {
            using var disposable = await assetProvider.LoadAssetAsync<TAsset>();
            var result = toFunc(disposable.Result);
            return result;
        }

        private async UniTask<TAsset> LoadAndAddToDisposablesAsync<TAsset>()
        {
            var disposable = await assetProvider.LoadAssetAsync<TAsset>();
            assetDisposables.Add(disposable);
            return disposable.Result;
        }

        private async UniTaskVoid DownloadAsync(string assetName, Func<UniTask> nextFunc)
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

                nextFunc?.Invoke().Forget();
            }
        }

        private async UniTaskVoid DownloadOrNotifyErrorAsync(string assetName, Func<UniTask> nextFunc)
            => await assetProvider.DownloadAsync(assetName, nextFunc: nextFunc);

        public UniTask<AssetDisposable<T>> LoadAssetAsync<T>(string assetName)
            => assetProvider.LoadAssetAsync<T>(assetName);

        public UniTask<AssetDisposable<SceneInstance>> LoadSceneAsync(string assetName)
            => assetProvider.LoadSceneAsync(assetName);
    }
}
