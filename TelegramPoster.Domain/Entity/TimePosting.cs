namespace TelegramPoster.Domain.Entity;

public class TimePosting
{
    public required Guid Id { get; set; }
    public required Guid DayId { get; set; }
    public required TimeOnly Time { get; set; }

    public Day? Day { get; set; }
}