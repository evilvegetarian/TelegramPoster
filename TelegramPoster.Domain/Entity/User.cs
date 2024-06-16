using System.Text.RegularExpressions;

namespace TelegramPoster.Domain.Entity;

public class User
{
    public required Guid Id { get; set; }
    public required string PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? TelegramUserName { get; set; }
}

public static class UserExtension
{
    public static bool IsEmail(this string UserName)
    {
        const string usernamePattern = "^[a-zA-Z0-9]*$";
        return Regex.IsMatch(UserName, usernamePattern);
    }
}