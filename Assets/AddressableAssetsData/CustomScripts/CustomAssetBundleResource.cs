using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using AsyncOperation = UnityEngine.AsyncOperation;

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
    private AssetBundleRequestOptions options;
    private string transformedInternalId;
    private string bundleFilePath;
    private const string Password = "password";

    private long bytesToDownload = -1;
    private long BytesToDownload
    {
        get
        {
            if (bytesToDownload == -1)
            {
                bytesToDownload = options?.ComputeSize(provideHandle.Location, provideHandle.ResourceManager) ?? 0;
            }
            return bytesToDownload;
        }
    }

    public void Setup(ProvideHandle handle)
    {
        provideHandle = handle;
        options = provideHandle.Location?.Data as AssetBundleRequestOptions;
        provideHandle.SetProgressCallback(GetProgress);
        provideHandle.SetDownloadProgressCallbacks(GetDownloadStatus);
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

        var relativeBundleFilePath
            = "Temp/com.unity.addressables/Encrypted/" + Path.GetFileName(transformedInternalId);
        bundleFilePath = Path.GetFullPath(relativeBundleFilePath);
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
            uwrAsyncOperation.webRequest.Dispose();
            uwrAsyncOperation = null;
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
#endif
            }
        }
    }

    [SuppressMessage("CodeCracker", "CC0022")]
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
            downloadHandler = new DownloadHandlerBuffer()
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
                return;
            }
            var data = uwr.downloadHandler.data;
            Decrypt(data);
            GetAssetBundleFromCacheOrFile();
        };
    }

    [SuppressMessage("CodeCracker", "CC0022")]
    private void Decrypt(byte[] encryptedData)
    {
        var bundleName = Path.GetFileNameWithoutExtension(transformedInternalId);
        using var encryptedStream = new MemoryStream(encryptedData);
        var uniqueSalt = Encoding.UTF8.GetBytes(bundleName);

        using var decryptor = new AesCbcStream(encryptedStream, Password, uniqueSalt, CryptoStreamMode.Read);
        var decryptedBuffer = new byte[decryptor.Length];
        _ = decryptor.Read(decryptedBuffer, 0, decryptedBuffer.Length);

        var bundleDirectoryPath = Path.GetDirectoryName(bundleFilePath);
        if (!Directory.Exists(bundleDirectoryPath))
        {
            _ = Directory.CreateDirectory(bundleDirectoryPath);
        }

        using var fileStream = new FileStream(bundleFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fileStream.Write(decryptedBuffer, 0, decryptedBuffer.Length);
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
            webRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleFilePath, cachedBundle, m_Options.Crc);
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

    public AssetBundle GetAssetBundle() => assetBundle;

    private float GetProgress() => uwrAsyncOperation?.webRequest.downloadProgress ?? 0.0f;
}

[System.ComponentModel.DisplayName("Custom AssetBundle Provider")]
public class CustomAssetBundleProvider : ResourceProviderBase
{
    public override void Provide(ProvideHandle providerInterface)
    {
        var res = new CustomAssetBundleResource();
        res.Setup(providerInterface);
        res.Fetch();
    }

    public override Type GetDefaultType(IResourceLocation location) => typeof(IAssetBundleResource);

    public override void Release(IResourceLocation location, object asset)
    {
        if (location == null)
        { throw new ArgumentNullException(nameof(location)); }

        if (asset == null)
        {
            Debug.LogWarningFormat("Releasing null asset bundle from location {0}.  This is an indication that the bundle failed to load.", location);
            return;
        }
        if (asset is CustomAssetBundleResource bundle)
        { bundle.Unload(); }
    }
}
