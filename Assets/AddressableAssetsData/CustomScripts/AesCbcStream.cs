using System.IO;
using System.Security.Cryptography;

/// <summary>
/// 暗号化/復号するStream
/// </summary>
public class AesCbcStream : Stream
{
    private readonly Stream baseStream;
    private readonly CryptoStreamMode mode;
    private readonly AesManaged aes;
    private readonly CryptoStream cryptoStream;

    private readonly byte[] iv = new byte[]
    {
        0x7D, 0xF1, 0xD1, 0xCC, 0xE4, 0x99, 0xE4, 0xB1, 0x87, 0x19, 0x3B, 0x2E, 0x93, 0xFD, 0x62, 0x9E
    };
    private const int StartOffset = 16;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="baseStream"></param>
    /// <param name="password"></param>
    /// <param name="salt">セキュリティのため、各Streamで異なるソルトを使う</param>
    public AesCbcStream(Stream baseStream, string password, byte[] salt, CryptoStreamMode mode)
    {
        this.baseStream = baseStream;
        this.mode = mode;
        using var key = new Rfc2898DeriveBytes(password, salt);
        aes = new AesManaged
        {
            BlockSize = 128,
            KeySize = 128,
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7,
            Key = key.GetBytes(16)
        };

        if (mode == CryptoStreamMode.Write)
        {
            aes.GenerateIV();
            var saveIv = aes.IV;
            aes.IV = iv;
            cryptoStream = new CryptoStream(baseStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(saveIv, 0, StartOffset);
        }
        else
        {
            aes.IV = iv;
            cryptoStream = new CryptoStream(baseStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            var _ = new byte[16];
            cryptoStream.Read(_, 0, StartOffset);
        }
    }

    public override bool CanRead => baseStream.CanRead && mode == CryptoStreamMode.Read;
    public override bool CanSeek => baseStream.CanSeek;
    public override bool CanWrite => baseStream.CanWrite && mode == CryptoStreamMode.Write;
    public override long Length => baseStream.Length - StartOffset;

    public override long Position
    {
        get => baseStream.Position - StartOffset;
        set => baseStream.Position = value + StartOffset;
    }

    public override void Flush() => baseStream.Flush();
    public override void SetLength(long value) => baseStream.SetLength(value + StartOffset);
    public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);

    public override int Read(byte[] buffer, int offset, int count)
        => cryptoStream.Read(buffer, offset, count);

    public override void Write(byte[] buffer, int offset, int count)
        => cryptoStream.Write(buffer, offset, count);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            cryptoStream?.Dispose();
            aes?.Dispose();
            baseStream?.Dispose();
        }

        base.Dispose(disposing);
    }
}
