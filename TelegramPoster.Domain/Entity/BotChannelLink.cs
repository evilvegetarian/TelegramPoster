namespace TelegramPoster.Domain.Entity;

public class BotChannelLink
{
    public required Guid BotId { get; init; }
    public required long ChannelId { get; init; }
    public TelegramBot Bot { get; init; }
}