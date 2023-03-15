using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Extreal.Integration.AssetWorkflow.Addressables.Custom.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Extreal.SampleApp.Holiday.App.AssetWorkflow.Custom
{
    public class CryptoStreamFactory : ICryptoStreamFactory
    {
        public CryptoStream CreateEncryptStream(Stream baseStream, AssetBundleRequestOptions options)
            => CreateCryptoStream(baseStream, options, CryptoStreamMode.Write);

        public CryptoStream CreateDecryptStream(Stream baseStream, AssetBundleRequestOptions options)
            => CreateCryptoStream(baseStream, options, CryptoStreamMode.Read);

        private static CryptoStream CreateCryptoStream
        (
            Stream baseStream,
            AssetBundleRequestOptions options,
            CryptoStreamMode mode
        )
        {
            using var aes = CreateAesManaged(options);
            var cryptor = mode == CryptoStreamMode.Write ? aes.CreateEncryptor() : aes.CreateDecryptor();
            return new CryptoStream(baseStream, cryptor, mode);
        }

        [SuppressMessage("Usage", "CC0022")]
        private static AesManaged CreateAesManaged(AssetBundleRequestOptions options)
        {
            const int keyLength = 128;
            var salt = Encoding.UTF8.GetBytes(options.BundleName);

            using var keyGen = new Rfc2898DeriveBytes(SecretVariables.CryptAssetPassword, salt, 100, HashAlgorithmName.SHA256);
            using var ivGen = new Rfc2898DeriveBytes(SecretVariables.CryptAssetIv, salt, 1, HashAlgorithmName.SHA256);

            var key = keyGen.GetBytes(keyLength / 8);
            var iv = ivGen.GetBytes(keyLength / 8);

            return new AesManaged
            {
                BlockSize = keyLength,
                KeySize = keyLength,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = key,
                IV = iv
            };
        }
    }
}
