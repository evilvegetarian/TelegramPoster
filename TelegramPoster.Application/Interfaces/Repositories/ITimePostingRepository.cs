using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Interfaces.Repositories;

public interface ITimePostingRepository
{
    Task AddAsync(TimePosting timePosting);
    Task AddAsync(List<TimePosting> timePostings);
    Task<List<TimePosting>> GetByDayIdsAsync(List<Guid> dayIds);
}