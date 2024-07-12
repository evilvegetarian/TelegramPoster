using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;
public interface IBotChannelRepository
{
    Task AddAsync(BotChannelLink link);
    Task<List<BotChannelLink>> GetListByBotIdAsync(Guid botId);
    Task<List<BotChannelLink>> GetListByBotIdsAsync(IEnumerable<Guid> botIds);
}