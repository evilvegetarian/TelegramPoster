using TelegramPoster.Application.Models.ScheduleTiming;

namespace TelegramPoster.Application.Services.ScheduleTimingServices;

public interface IScheduleTimingService
{
    Task CreateForDayOfWeek(ScheduleTimingDayOfWeekRequestForm createDayOfWeekSchedule);
    List<DayOfWeekResponseModel> GetAllDayOfWeek();
    Task<ScheduleTimingResponseModel> GetDayOfWeekTiming(Guid scheduleId);
}