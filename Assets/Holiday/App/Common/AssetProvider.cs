using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Extreal.SampleApp.Holiday.App.Common
{
    /// <summary>
    /// Extreal.Integration.Assets.Addressablesモジュールに入るクラス。
    /// </summary>
    [SuppressMessage("Design", "CC0091")]
    public class AssetProvider : DisposableBase
    {
        public IObservable<Unit> OnDownloading => onDownloading;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onDownloading = new Subject<Unit>();

        public IObservable<DownloadStatus> OnDownloaded => onDownloaded;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<DownloadStatus> onDownloaded = new Subject<DownloadStatus>();

        protected override void ReleaseManagedResources() => onDownloaded.Dispose();

        public async UniTask DownloadAsync(
            string assetName, TimeSpan downloadStatusInterval = default, Func<UniTask> nextFunc = null)
        {
            if (await GetDownloadSizeAsync(assetName) != 0)
            {
                await DownloadDependenciesAsync(assetName, downloadStatusInterval);
            }
            nextFunc?.Invoke();
        }

        public async UniTask<long> GetDownloadSizeAsync(string assetName)
        {
            var handle = Addressables.GetDownloadSizeAsync(assetName);
            var size = await handle.Task;
            ReleaseHandle(handle);
            return size;
        }

        private async UniTask DownloadDependenciesAsync(string assetName, TimeSpan interval = default)
        {
            onDownloading.OnNext(Unit.Default);

            var handle = Addressables.DownloadDependenciesAsync(assetName);

            onDownloaded.OnNext(handle.GetDownloadStatus());
            var downloadStatus = default(DownloadStatus);
            while (handle.Status == AsyncOperationStatus.None) // None: the operation is still in progress.
            {
                var prevDownloadStatus = downloadStatus;
                downloadStatus = handle.GetDownloadStatus();
                if (prevDownloadStatus.DownloadedBytes != downloadStatus.DownloadedBytes)
                {
                    onDownloaded.OnNext(downloadStatus);
                }
                if (interval == default)
                {
                    await UniTask.Yield();
                }
                else
                {
                    await UniTask.Delay(interval);
                }
            }
            onDownloaded.OnNext(handle.GetDownloadStatus());

            ReleaseHandle(handle);
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName)
        {
            var handle = Addressables.LoadAssetAsync<T>(assetName);
            var asset = await handle.Task;
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                ReleaseHandle(handle);
            }
            return asset;
        }

        public async UniTask<SceneInstance> LoadSceneAsync(string assetName, LoadSceneMode loadMode = LoadSceneMode.Additive)
        {
            var handle = Addressables.LoadSceneAsync(assetName, loadMode);
            var scene = await handle.Task;
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                ReleaseHandle(handle);
            }
            return scene;
        }

        private static void ReleaseHandle(AsyncOperationHandle handle)
        {
            var exception = handle.OperationException;
            Addressables.Release(handle);
            if (exception != null)
            {
                ExceptionDispatchInfo.Throw(exception);
            }
        }
    }
}
