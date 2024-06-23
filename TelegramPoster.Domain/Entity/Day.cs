namespace TelegramPoster.Domain.Entity;

public class Day
{
    public required Guid Id { get; init; }
    public required Guid ScheduleId { get; init; }
    public DayOfWeek? DayOfWeek { get; init; }
    public DateOnly? DateDay { get; init; }


    public Schedule? Schedule { get; set; }
    public ICollection<TimePosting> TimePostings { get; set; } = [];
}