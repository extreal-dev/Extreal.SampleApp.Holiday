using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Extreal.SampleApp.Holiday.App.Data
{
    public interface IDataLoader
    {
        IObservable<(string name, DownloadStatus downloadStatus)> OnDownloadStatusChanged { get; }
        UniTask<long> GetDownloadSizeAsync(string name);
        UniTask DownloadAsync<T>(string name, TimeSpan interval = default);
        UniTask<T> LoadAssetAsync<T>(string name);
        void ReleaseAsset(Object obj);
        void ReleaseAssets(IEnumerable<Object> objs);
    }
}
