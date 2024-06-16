using Dapper;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Persistence.Repositories;

public class TelegramBotRepository(ISqlConnectionFactory connection) : ITelegramBotRepository
{
    public async Task AddAsync(TelegramBot telegramBot)
    {
        const string sql = """
                           INSERT INTO "TelegramBot" ("Id", "UserId", "ApiTelegram", "BotStatus", "ChatIdWithBotUser", "NameBot")
                           VALUES (@Id, @UserId, @ApiTelegram, @BotStatus, @ChatIdWithBotUser, @NameBot)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, telegramBot);
    }
    public async Task<List<TelegramBot>> GetAsync()
    {
        const string sql = """
                           SELECT * FROM "TelegramBot"
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<TelegramBot>(sql)).ToList();
    }

    public async Task<TelegramBot?> GetAsync(Guid id)
    {
        const string sql = """
                           SELECT * FROM "TelegramBot"
                           WHERE "Id"=@Id
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<TelegramBot>(sql, new { Id = id });
    }

    public async Task<List<TelegramBot>> GetByUserIdAsync(Guid userId)
    {
        const string sql = """
                           SELECT * FROM "TelegramBot"
                           WHERE "UserId"=@UserId
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<TelegramBot>(sql, new { UserId = userId })).ToList();
    }

    public async Task<List<TelegramBot>> GetByStatusAsync(BotStatus botStatus)
    {
        const string sql = """
                           SELECT * FROM "TelegramBot"
                           WHERE "BotStatus"=@BotStatus
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<TelegramBot>(sql, new { BotStatus = botStatus })).ToList();
    }


}