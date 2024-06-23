using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Interfaces.Repositories;

public interface IDayRepository
{
    Task AddAsync(Day day);
    Task AddListAsync(List<Day> days);
    Task<Day?> GetAsync(Guid id);
    Task<List<Day>> GetListByScheduleIdAsync(Guid scheduleId);
    Task<List<Day>> GetListWithTimeByScheduleIdAsync(Guid scheduleId);
}