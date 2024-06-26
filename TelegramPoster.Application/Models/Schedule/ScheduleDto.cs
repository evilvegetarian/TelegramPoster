namespace TelegramPoster.Application.Models.Schedule;

public class ScheduleDto
{
    public required string Name { get; init; }
    public required long ChannelId { get; init; }
    public required Guid BotId { get; init; }
}