using Dapper;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class BotChannelRepository(ISqlConnectionFactory connection) : IBotChannelRepository
{
    public async Task AddAsync(BotChannelLink link)
    {
        const string sql = """
                           INSERT INTO "BotChannelLink" ("BotId", "ChannelId")
                           VALUES (@BotId, @ChannelId)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, link);
    }

    public async Task<List<BotChannelLink>> GetListByBotIdAsync(Guid botId)
    {
        const string sql = """
                           SELECT * FROM "BotChannelLink"
                           WHERE "BotId"=@BotId
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<BotChannelLink>(sql, new { BotId = botId })).ToList();
    }

    public async Task<List<BotChannelLink>> GetListByBotIdsAsync(IEnumerable<Guid> botIds)
    {
        const string sql = """
                           SELECT * FROM "BotChannelLink"
                           WHERE "BotId" = ANY(@BotIds)
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<BotChannelLink>(sql, new { BotIds = botIds.ToList() })).ToList();
    }
}