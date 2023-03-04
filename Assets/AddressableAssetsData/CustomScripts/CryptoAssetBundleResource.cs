using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using AsyncOperation = UnityEngine.AsyncOperation;


namespace MyCustom
{
    public class CustomAssetBundleResource : IAssetBundleResource
    {
        private enum LoadType
        {
            None,
            Local,
            Web
        }

        private AssetBundle assetBundle;
        private UnityWebRequestAsyncOperation uwrAsyncOperation;
        private ProvideHandle provideHandle;
        private readonly AssetBundleRequestOptions options;
        private readonly ICryptoStreamGetter cryptoStreamGetter;
        private string transformedInternalId;
        private string downloadFilePath;
        private string bundleFilePath;

        private long bytesToDownload = -1;
        private long BytesToDownload
        {
            get
            {
                if (bytesToDownload == -1)
                {
                    bytesToDownload
                        = options?.ComputeSize(provideHandle.Location, provideHandle.ResourceManager) ?? 0L;
                }
                return bytesToDownload;
            }
        }

        public CustomAssetBundleResource(ProvideHandle provideHandle, ICryptoStreamGetter cryptoStreamGetter)
        {
            this.provideHandle = provideHandle;
            this.cryptoStreamGetter = cryptoStreamGetter;
            this.provideHandle.SetProgressCallback(GetProgress);
            this.provideHandle.SetDownloadProgressCallbacks(GetDownloadStatus);
            options = this.provideHandle.Location?.Data as AssetBundleRequestOptions;
        }

        private DownloadStatus GetDownloadStatus()
        {
            if (options == null)
            {
                return default;
            }

            var status = new DownloadStatus
            {
                TotalBytes = BytesToDownload,
                IsDone = GetProgress() >= 1f
            };

            var downloadedBytes = 0L;
            if (BytesToDownload > 0 && uwrAsyncOperation != null
                && string.IsNullOrEmpty(uwrAsyncOperation.webRequest.error))
            {
                downloadedBytes = (long)uwrAsyncOperation.webRequest.downloadedBytes;
            }

            status.DownloadedBytes = downloadedBytes;
            return status;
        }

        public void Fetch()
        {
            GetLoadInfo(provideHandle, out var loadType, out transformedInternalId);
            if (loadType == LoadType.Local)
            {
                var requestOperation = AssetBundle.LoadFromFileAsync(transformedInternalId, options?.Crc ?? 0);
                AddCallbackInvokeIfDone(requestOperation, RequestOperationToGetAssetBundleCompleted);
            }
            else if (loadType == LoadType.Web)
            {
                CreateAndSendWebRequest(transformedInternalId);
            }
            else
            {
                var exception = new RemoteProviderException
                (
                    $"Invalid path in AssetBundleProvider: '{transformedInternalId}'.",
                    provideHandle.Location
                );
                provideHandle.Complete<CustomAssetBundleResource>(null, false, exception);
            }
        }

        private void GetLoadInfo(ProvideHandle handle, out LoadType loadType, out string path)
        {
            if (options == null)
            {
                loadType = LoadType.None;
                path = null;
                return;
            }

            path = handle.ResourceManager.TransformInternalId(handle.Location);
            if (ResourceManagerConfig.ShouldPathUseWebRequest(path))
            {
                loadType = LoadType.Web;
            }
            else if (options.UseUnityWebRequestForLocalBundles)
            {
                path = "file:///" + Path.GetFullPath(path);
                loadType = LoadType.Web;
            }
            else
            {
                loadType = LoadType.Local;
            }

            downloadFilePath = Path.GetFullPath("Temp/com.unity.addressables/Encrypted/" + Path.GetFileName(path));
            bundleFilePath = Path.GetFullPath("Temp/com.unity.addressables/Decrypted/" + Path.GetFileName(path));
        }

        private static void AddCallbackInvokeIfDone(AsyncOperation operation, Action<AsyncOperation> callback)
        {
            if (operation.isDone)
            {
                callback?.Invoke(operation);
            }
            else
            {
                operation.completed += callback;
            }
        }

        private void RequestOperationToGetAssetBundleCompleted(AsyncOperation op)
        {
            if (op is AssetBundleCreateRequest assetBundleCreateRequest)
            {
                CompleteBundleLoad(assetBundleCreateRequest.assetBundle);
            }
            else if (op is UnityWebRequestAsyncOperation uwrAsyncOp
                        && uwrAsyncOp.webRequest.downloadHandler is DownloadHandlerAssetBundle dhAssetBundle)
            {
                CompleteBundleLoad(dhAssetBundle.assetBundle);
                uwrAsyncOp.webRequest.Dispose();
                uwrAsyncOperation?.webRequest.Dispose();
                uwrAsyncOperation = null;

                if (File.Exists(downloadFilePath))
                {
                    File.Delete(downloadFilePath);
                }
                if (File.Exists(bundleFilePath))
                {
                    File.Delete(bundleFilePath);
                }
            }
        }

        private void CompleteBundleLoad(AssetBundle bundle)
        {
            assetBundle = bundle;
            if (assetBundle != null)
            {
                provideHandle.Complete(this, true, null);

#if ENABLE_CACHING
                if (!string.IsNullOrEmpty(options.Hash) && options.ClearOtherCachedVersionsWhenLoaded)
                {
                    Caching.ClearOtherCachedVersions(options.BundleName, Hash128.Parse(options.Hash));
                }
#endif
            }
            else
            {
                var exception = new RemoteProviderException
                (
                    $"Invalid path in AssetBundleProvider: '{transformedInternalId}'.",
                    provideHandle.Location
                );
                provideHandle.Complete<CustomAssetBundleResource>(null, false, exception);

#if ENABLE_CACHING
                if (!string.IsNullOrEmpty(options.Hash))
                {
                    var cab = new CachedAssetBundle(options.BundleName, Hash128.Parse(options.Hash));
                    if (Caching.IsVersionCached(cab))
                    {
                        Caching.ClearCachedVersion(cab.name, cab.hash);
                    }
                }
#endif
            }
        }

        private void CreateAndSendWebRequest(string path)
        {
            if (Caching.IsVersionCached(new CachedAssetBundle(options.BundleName, Hash128.Parse(options.Hash))))
            {
                GetAssetBundleFromCacheOrFile();
                return;
            }

            var uwr = new UnityWebRequest(path)
            {
                disposeDownloadHandlerOnDispose = true,
                downloadHandler = new DownloadHandlerFile(downloadFilePath)
            };

            if (options.RedirectLimit > 0)
            {
                uwr.redirectLimit = options.RedirectLimit;
            }
            if (provideHandle.ResourceManager.CertificateHandlerInstance != null)
            {
                uwr.certificateHandler = provideHandle.ResourceManager.CertificateHandlerInstance;
                uwr.disposeCertificateHandlerOnDispose = false;
            }
            provideHandle.ResourceManager.WebRequestOverride?.Invoke(uwr);

            uwrAsyncOperation = uwr.SendWebRequest();
            uwrAsyncOperation.completed += op =>
            {
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    var exception = new RemoteProviderException
                    (
                        $"Download has failed. result:{uwr.result} path:{path}",
                        provideHandle.Location
                    );
                    provideHandle.Complete<CustomAssetBundleResource>(null, false, exception);
                    return;
                }
                Decrypt(downloadFilePath);
                GetAssetBundleFromCacheOrFile();
            };
        }

        private void Decrypt(string downloadFilePath)
        {
            var bundleDirectoryPath = Path.GetDirectoryName(bundleFilePath);
            if (!Directory.Exists(bundleDirectoryPath))
            {
                _ = Directory.CreateDirectory(bundleDirectoryPath);
            }

            using var srcStream = new FileStream(downloadFilePath, FileMode.Open, FileAccess.Read);
            using var decryptor = cryptoStreamGetter.GetDecryptStream(srcStream, options);
            using var destStream = new FileStream(bundleFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            decryptor.CopyTo(destStream);
        }

        private void GetAssetBundleFromCacheOrFile()
        {
            var localBundleFilePath = "file:///" + bundleFilePath;
            UnityWebRequest uwr;
            if (!string.IsNullOrEmpty(options.Hash))
            {
                var cachedBundle = new CachedAssetBundle(options.BundleName, Hash128.Parse(options.Hash));
#if ENABLE_CACHING
                uwr = options.UseCrcForCachedBundle || !Caching.IsVersionCached(cachedBundle)
                    ? UnityWebRequestAssetBundle.GetAssetBundle(localBundleFilePath, cachedBundle, options.Crc)
                    : UnityWebRequestAssetBundle.GetAssetBundle(localBundleFilePath, cachedBundle);
#else
                webRequest = UnityWebRequestAssetBundle.GetAssetBundle(localBundleFilePath, cachedBundle, m_Options.Crc);
#endif
            }
            else
            {
                uwr = UnityWebRequestAssetBundle.GetAssetBundle(localBundleFilePath, options.Crc);
            }

            var uwrAsyncOp = uwr.SendWebRequest();
            AddCallbackInvokeIfDone(uwrAsyncOp, RequestOperationToGetAssetBundleCompleted);
        }

        public void Unload()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
        }

        public AssetBundle GetAssetBundle()
            => assetBundle;

        private float GetProgress()
            => uwrAsyncOperation?.webRequest.downloadProgress ?? 0f;
    }
}
