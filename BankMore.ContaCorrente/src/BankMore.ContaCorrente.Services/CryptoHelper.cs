using System.Security.Cryptography;
using System.Text;

namespace BankMore.ContaCorrente.Services;

public static class CryptoHelper
{
    public static string CreateSalt()
    {
        var buffer = new byte[8];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }

    private static readonly byte[] internalKey = new byte[] { 148, 40, 202, 95, 187, 229, 104, 154, 211, 66, 168, 110, 35, 23, 37, 175 };
    private const int Rfc2898KeygenIterations = 100;
    private const int AesKeySizeInBits = 128;

    private static Aes CreateAes(byte[] saltBytes)
    {
        var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.KeySize = AesKeySizeInBits;
        var keyStrengthInBytes = aes.KeySize / 8;

#pragma warning disable S5344 // Passwords should not be stored in plaintext or with a fast hashing algorithm
        var rfc2898 = new Rfc2898DeriveBytes(internalKey, saltBytes, Rfc2898KeygenIterations, HashAlgorithmName.SHA1);
#pragma warning restore S5344 // Passwords should not be stored in plaintext or with a fast hashing algorithm
        aes.Key = rfc2898.GetBytes(keyStrengthInBytes);
        aes.IV = rfc2898.GetBytes(keyStrengthInBytes);

        return aes;
    }

    public static string Encrypt(string input, string salt)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var saltBytes = Encoding.UTF8.GetBytes(salt);

        using var aes = CreateAes(saltBytes);

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            cs.Write(inputBytes, 0, inputBytes.Length);

        var encodedBytes = ms.ToArray();
        return Convert.ToBase64String(encodedBytes);
    }

    public static string Decrypt(string encrypted, string salt)
    {
        var encodedBytes = Convert.FromBase64String(encrypted);
        var saltBytes = Encoding.UTF8.GetBytes(salt);

        using var aes = CreateAes(saltBytes);

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            cs.Write(encodedBytes, 0, encodedBytes.Length);

        var decodedBytes = ms.ToArray();

        return Encoding.UTF8.GetString(decodedBytes);
    }
}
