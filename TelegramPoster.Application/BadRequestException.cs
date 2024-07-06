namespace TelegramPoster.Application;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string name, string message) : base($"{name}: {message}") { }
}