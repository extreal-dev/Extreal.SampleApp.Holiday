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
        public IObservable<DownloadStatus> OnDownloading => onDownloading;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<DownloadStatus> onDownloading = new Subject<DownloadStatus>();

        protected override void ReleaseManagedResources() => onDownloading.Dispose();

        public async UniTask DownloadAsync(
            string assetName, TimeSpan downloadStatusInterval = default, Action nextAction = null)
        {
            if (await GetDownloadSizeAsync(assetName) == 0)
            {
                await DownloadDependenciesAsync(assetName, downloadStatusInterval);
            }
            nextAction?.Invoke();
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
            var handle = Addressables.DownloadDependenciesAsync(assetName);

            var isFirst = true;
            var downloadStatus = default(DownloadStatus);
            while (!handle.IsDone && !downloadStatus.IsDone)
            {
                var prevDownloadStatus = downloadStatus;
                downloadStatus = handle.GetDownloadStatus();
                if (isFirst)
                {
                    isFirst = false;
                    onDownloading.OnNext(downloadStatus);
                }
                else if (prevDownloadStatus.DownloadedBytes != downloadStatus.DownloadedBytes)
                {
                    onDownloading.OnNext(downloadStatus);
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

        public T LoadAsset<T>(string assetName)
        {
            var handle = Addressables.LoadAssetAsync<T>(assetName);
            var asset = handle.WaitForCompletion();
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

        public SceneInstance LoadScene(string assetName, LoadSceneMode loadMode = LoadSceneMode.Additive)
        {
            var handle = Addressables.LoadSceneAsync(assetName, loadMode);
            var scene = handle.WaitForCompletion();
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
