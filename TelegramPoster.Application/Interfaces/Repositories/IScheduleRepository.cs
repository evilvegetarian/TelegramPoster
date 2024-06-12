using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Interfaces.Repositories;

public interface IScheduleRepository
{
    Task AddAsync(Schedule schedule);
    Task<Schedule?> GetAsync(Guid id);
    Task<List<Schedule>> GetListByUserIdAndChannelAsync(Guid userId, Guid channelId);
    Task<List<Schedule>> GetListByUserIdAsync(Guid userId);
    Task UpdateNameAsync(Guid id, string name);
}