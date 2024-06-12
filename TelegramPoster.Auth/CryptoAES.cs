using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using TelegramPoster.Auth.Interface;

namespace TelegramPoster.Auth;

public class CryptoAES : ICryptoAES
{
    private readonly Aes aes;
    private readonly TelegramOptions options;

    public CryptoAES(IOptions<TelegramOptions> options)
    {
        this.options = options.Value;
        aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(this.options.SecretKey);
        aes.IV = new byte[16];
    }

    public string Encrypt(string plainText)
    {
        byte[] encrypted;
        using (var memoryStream = new MemoryStream())
        {
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                }
                encrypted = memoryStream.ToArray();
            }
        }
        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string cipherText)
    {
        string? plaintext = null;
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (var memoryStream = new MemoryStream(cipherBytes))
        {
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using (var reader = new StreamReader(cryptoStream))
                {
                    plaintext = reader.ReadToEnd();
                }
            }
        }
        return plaintext;
    }
}

public class TelegramOptions
{
    public required string SecretKey { get; set; }
}
