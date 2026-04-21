using System.Security.Cryptography;
using NetPC.Application.Encryption;

namespace NetPC.Infrastructure.Encryption;

/// <summary>
/// Service for encrypting and decrypting Contact password
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(string hexKey)
    {
        _key = Convert.FromHexString(hexKey);
        if (_key.Length != 32)
            throw new ArgumentException("Encryption key must be 32 bytes.");
    }

    public string Encrypt(string plainText)
    {
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        var plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(_key, 16);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // nonce (12) + tag (16) + cipher
        var result = new byte[12 + 16 + cipherBytes.Length];
        nonce.CopyTo(result, 0);
        tag.CopyTo(result, 12);
        cipherBytes.CopyTo(result, 28);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);

        var nonce = data[..12];
        var tag = data[12..28];
        var cipherBytes = data[28..];
        var plainBytes = new byte[cipherBytes.Length];

        using var aes = new AesGcm(_key, 16);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return System.Text.Encoding.UTF8.GetString(plainBytes);
    }
}
