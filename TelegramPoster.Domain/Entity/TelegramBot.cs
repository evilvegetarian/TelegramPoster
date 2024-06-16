using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Domain.Entity;

public class TelegramBot
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string ApiTelegram { get; set; }
    public required string NameBot { get; set; }
    public long ChatIdWithBotUser { get; set; }
    public required BotStatus BotStatus { get; set; }


    public User? User { get; set; }
}
