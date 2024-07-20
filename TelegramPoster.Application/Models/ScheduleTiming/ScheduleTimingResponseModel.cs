using System.ComponentModel.DataAnnotations;

namespace TelegramPoster.Application.Models.ScheduleTiming;

public class ScheduleTimingResponseModel
{
    [Required]
    public required List<ScheduleTimingDayOfWeekResponseModel> DayOfWeekModels { get; set; } = [];
}

public class ScheduleTimingDayOfWeekResponseModel
{
    [Required]
    public required DayOfWeek DayOfWeekPosting { get; set; }

    [Required]
    public required List<TimeOnly> TimePosting { get; set; } = [];
}