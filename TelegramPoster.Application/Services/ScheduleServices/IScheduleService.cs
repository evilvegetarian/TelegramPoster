using TelegramPoster.Application.Models.Schedule;

namespace TelegramPoster.Application.Services.ScheduleServices;

public interface IScheduleService
{
    Task CreateAsync(ScheduleDto scheduleDto);
    Task<List<ScheduleView>> GetAllAsync();
}