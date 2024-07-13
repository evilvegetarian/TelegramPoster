using TelegramPoster.Application.Models.Schedule;

namespace TelegramPoster.Application.Services.ScheduleServices;

public interface IScheduleService
{
    Task CreateAsync(ScheduleCreateRequestForm scheduleDto);
    Task<List<ScheduleResponseModel>> GetAllAsync();
}