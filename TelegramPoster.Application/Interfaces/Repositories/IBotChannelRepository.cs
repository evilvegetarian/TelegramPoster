using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;
public interface IBotChannelRepository
{
    Task AddAsync(BotChannelLink link);
    Task AddAsync(List<BotChannelLink> links);
    Task<List<BotChannelLink>> GetListByBotIdAsync(Guid botId);
    Task<List<BotChannelLink>> GetListByBotIdsAsync(List<Guid> botIds);
}