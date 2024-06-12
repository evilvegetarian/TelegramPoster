namespace TelegramPoster.Domain.Entity;

public class TelegramBot
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string ApiTelegram { get; set; }
    public long? ChatIdWithBotUser { get; set; }
    public required BotStatus BotStatus { get; set; }

    public User User { get; set; }
}

public enum BotStatus
{
    Register = 0,
    InHandle = 1,

    Delete = 8,
    Error = 10
}