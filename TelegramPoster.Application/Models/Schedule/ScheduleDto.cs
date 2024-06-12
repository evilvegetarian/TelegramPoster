namespace TelegramPoster.Application.Models.Schedule;

public class ScheduleDto
{
    public required string Name { get; init; }
    public long ChannelId { get; init; }
}