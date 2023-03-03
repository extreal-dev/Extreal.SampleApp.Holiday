using System.IO;
using System.Security.Cryptography;
using UnityEngine.ResourceManagement.ResourceProviders;

public interface ICryptoStreamGetter
{
    CryptoStream GetEncryptStream(Stream baseStream, AssetBundleRequestOptions options);
    CryptoStream GetDecryptStream(Stream baseStream, AssetBundleRequestOptions options);
}
