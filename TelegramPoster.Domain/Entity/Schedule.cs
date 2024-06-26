namespace TelegramPoster.Domain.Entity;

public class Schedule
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required long ChannelId { get; init; }
    public required Guid BotId { get; init; }

    public TelegramBot? TelegramBot { get; set; }
    public List<Day> Days { get; set; } = [];
    public User? User { get; set; }
}