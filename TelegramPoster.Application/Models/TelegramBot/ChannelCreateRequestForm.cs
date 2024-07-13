namespace TelegramPoster.Application.Models.TelegramBot;

public class ChannelCreateRequestForm
{
    public required Guid BotId { get; init; }
    public required string Channel { get; init; }
}