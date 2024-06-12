using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class ScheduleRepository(ISqlConnectionFactory connection) : IScheduleRepository
{
    public async Task AddAsync(Schedule schedule)
    {
        const string sql = """
                           INSERT INTO "Schedule" ("Id", "UserId", "Name", "ChannelId")
                           VALUES (@Id, @UserId, @Name, @ChannelId)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, schedule);
    }

    public async Task<Schedule?> GetAsync(Guid id)
    {
        const string sql = """
                           SELECT * FROM "Schedule"
                           WHERE "Id"=@Id
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<Schedule>(sql, new { Id = id });
    }

    public async Task<List<Schedule>> GetListByUserIdAsync(Guid userId)
    {
        const string sql = """
                           SELECT * FROM "Schedule"
                           WHERE "UserId"=@UserId
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<Schedule>(sql, new { UserId = userId })).ToList();
    }

    public async Task UpdateNameAsync(Guid id, string name)
    {
        const string sql = """
                           UPDATE "Schedule"
                           SET "Name"=@Name
                           WHERE "Id"=@Id
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, new { Name = name, Id = id });
    }

    public async Task<List<Schedule>> GetListByUserIdAndChannelAsync(Guid userId, Guid channelId)
    {
        string sql = """
                     SELECT * FROM "Schedule"
                     WHERE "UserId"=@UserId
                     AND "ChannelId"=@ChannelId
                     """;

        using var db = connection.Create();
        return (await db.QueryAsync<Schedule>(sql, new { UserId = userId, ChannelId = channelId })).ToList();
    }
}
