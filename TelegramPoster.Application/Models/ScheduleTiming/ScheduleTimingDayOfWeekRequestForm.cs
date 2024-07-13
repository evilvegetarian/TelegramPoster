using System.ComponentModel.DataAnnotations;

namespace TelegramPoster.Application.Models.ScheduleTiming;

public class ScheduleTimingDayOfWeekRequestForm
{
    [Required]
    public required Guid ScheduleId { get; set; }

    [Required]
    public required List<DayOfWeekRequestForm> DayOfWeekForms { get; set; } = [];
}

public class DayOfWeekRequestForm
{
    [Required]
    public required DayOfWeek DayOfWeekPosting { get; set; }

    [Required]
    public required TimeOnly StartPosting { get; set; }

    [Required]
    public required TimeOnly EndPosting { get; set; }

    [Required]
    public required byte Interval { get; set; }
}