using System.Runtime.ExceptionServices;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UniRx;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using Extreal.Core.Common.System;
using System.Diagnostics.CodeAnalysis;

namespace Extreal.SampleApp.Holiday.App.Data
{
    public class AddressablesDataLoader : DisposableBase, IDataLoader
    {
        public IObservable<(string name, DownloadStatus downloadStatus)> OnDownloadStatusChanged
            => onDownloadStatusChanged;
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly ReactiveProperty<(string name, DownloadStatus downloadStatus)> onDownloadStatusChanged
            = new ReactiveProperty<(string, DownloadStatus)>();

        protected override void ReleaseManagedResources()
            => onDownloadStatusChanged.Dispose();

        public async UniTask<long> GetDownloadSizeAsync(string name)
        {
            var opHandle = Addressables.GetDownloadSizeAsync(name);
            var size = await opHandle.Task;
            var exception = opHandle.OperationException;

            Addressables.Release(opHandle);

            if (exception != null)
            {
                ExceptionDispatchInfo.Throw(exception);
            }

            return size;
        }

        public async UniTask DownloadAsync<T>(string name, TimeSpan interval = default)
        {
            var opHandle = Addressables.LoadAssetAsync<T>(name);
            var downloadStatus = default(DownloadStatus);
            while (!opHandle.IsDone && downloadStatus.Percent < 1f)
            {
                downloadStatus = opHandle.GetDownloadStatus();
                if (downloadStatus.Percent != onDownloadStatusChanged.Value.downloadStatus.Percent)
                {
                    onDownloadStatusChanged.Value = (name, downloadStatus);
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
            onDownloadStatusChanged.Value = (name, opHandle.GetDownloadStatus());

            var exception = opHandle.OperationException;

            Addressables.Release(opHandle);

            if (exception != null)
            {
                ExceptionDispatchInfo.Throw(exception);
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(string name)
        {
            var opHandle = Addressables.LoadAssetAsync<T>(name);
            var asset = await opHandle.Task;
            if (opHandle.Status == AsyncOperationStatus.Failed)
            {
                var exception = opHandle.OperationException;
                Addressables.Release(opHandle);
                ExceptionDispatchInfo.Throw(exception);
            }

            return asset;
        }

        public void ReleaseAsset(Object obj)
            => Addressables.Release(obj);

        public void ReleaseAssets(IEnumerable<Object> objs)
        {
            foreach (var obj in objs)
            {
                ReleaseAsset(obj);
            }
        }
    }
}
