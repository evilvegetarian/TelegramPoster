using System.ComponentModel.DataAnnotations;

namespace TelegramPoster.Application.Models.Day;

public class DayOfWeekScheduleForm
{
    [Required]
    public required Guid ScheduleId { get; set; }

    [Required]
    public required List<DayOfWeekForm> DayOfWeekForms { get; set; } = [];
}

public class DayOfWeekForm
{
    [Required]
    public required DayOfWeek DayOfWeekPosting { get; set; }

    [Required]
    public required bool IsInterval { get; set; }

    public DayOfWeekScheduleIntervalForm? CreateDayScheduleIntervalForms { get; set; }

    public List<TimeOnly> TimesPosting { get; set; } = [];
}

public class DayOfWeekScheduleIntervalForm
{
    [Required]
    public required TimeOnly StartPosting { get; set; }

    [Required]
    public required TimeOnly EndPosting { get; set; }

    [Required]
    public required byte Interval { get; set; }
}