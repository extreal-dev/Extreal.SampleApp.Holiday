using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AesCbcStreamGetter : ICryptoStreamGetter
{
    private readonly byte[] iv = new byte[]
    {
        0x7D, 0xF1, 0xD1, 0xCC, 0xE4, 0x99, 0xE4, 0xB1, 0x87, 0x19, 0x3B, 0x2E, 0x93, 0xFD, 0x62, 0x9E
    };
    private const string Password = "password";
    private const int StartOffset = 16;

    public CryptoStream GetEncryptStream(Stream baseStream, AssetBundleRequestOptions options)
    {
        using var aes = CreateAesManaged(options);

        aes.GenerateIV();
        var saveIv = aes.IV;
        aes.IV = iv;
        var encryptStream = new CryptoStream(baseStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        encryptStream.Write(saveIv, 0, StartOffset);

        return encryptStream;
    }

    public CryptoStream GetDecryptStream(Stream baseStream, AssetBundleRequestOptions options)
    {
        using var aes = CreateAesManaged(options);

        aes.IV = iv;
        var decryptStream = new CryptoStream(baseStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        var _ = new byte[16];
        decryptStream.Read(_, 0, StartOffset);

        return decryptStream;
    }

    [SuppressMessage("CodeCracker", "CC0022")]
    private static AesManaged CreateAesManaged(AssetBundleRequestOptions options)
    {
        var salt = Encoding.UTF8.GetBytes(options.BundleName);
        using var key = new Rfc2898DeriveBytes(Password, salt);
        return new AesManaged
        {
            BlockSize = 128,
            KeySize = 128,
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7,
            Key = key.GetBytes(16)
        };
    }
}
