using TelegramPoster.Application.Models.Day;

namespace TelegramPoster.Application.Services.DayServices;

public interface IDayService
{
    Task CreateForDayOfWeek(ScheduleTimingDayOfWeekForm createDayOfWeekSchedule);
    List<DayOfWeekViewModel> GetAllDayOfWeek();
}