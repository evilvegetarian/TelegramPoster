using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class TimePostingRepository(ISqlConnectionFactory connection) : ITimePostingRepository
{
    public async Task AddAsync(TimePosting timePosting)
    {
        const string sql = """
                           INSERT INTO "TimePosting" ("Id", "DayId", "Time")
                           VALUES (@Id, @DayId, @Time)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, timePosting);
    }

    public async Task AddAsync(List<TimePosting> timePostings)
    {
        const string sql = """
                           INSERT INTO "TimePosting" ("Id", "DayId", "Time")
                           VALUES (@Id, @DayId, @Time)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, timePostings);
    }

    public async Task<List<TimePosting>> GetByDayIdsAsync(List<Guid> dayIds)
    {
        const string sql = """
                           SELECT * FROM "TimePosting"
                           WHERE "DayId" = ANY(@Ids)
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<TimePosting>(sql, new { Ids = dayIds })).ToList();
    }
}