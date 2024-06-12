namespace TelegramPoster.Auth.Interface;

public interface ICryptoAES
{
    string Decrypt(string cipherText);
    string Encrypt(string plainText);
}