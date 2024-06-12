using TelegramPoster.Application.Models.Day;

namespace TelegramPoster.Application.Services.DayServices;

public interface IDayService
{
    Task CreateForDayOfWeek(DayOfWeekScheduleForm createDayOfWeekSchedule);
    List<DayOfWeekViewModel> GetAllDayOfWeek();
}