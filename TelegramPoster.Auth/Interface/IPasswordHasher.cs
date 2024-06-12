namespace TelegramPoster.Auth.Interface;

public interface IPasswordHasher
{
    bool CheckPassword(string password, string passwordHash);
    string Generate(string password);
}