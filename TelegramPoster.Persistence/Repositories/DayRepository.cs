using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class DayRepository(ISqlConnectionFactory connection) : IDayRepository
{
    public async Task AddAsync(Day day)
    {
        const string sql = """
                           INSERT INTO "Days" ("Id", "ScheduleId", "DayOfWeek", "DateDay")
                           VALUES (@Id, @ScheduleId, @DayOfWeek, @DateDay)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, day);
    }

    public async Task AddListAsync(List<Day> days)
    {
        const string sql = """
                           INSERT INTO "Days" ("Id", "ScheduleId", "DayOfWeek", "DateDay")
                           VALUES (@Id, @ScheduleId, @DayOfWeek, @DateDay)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, days);
    }

    public async Task<Day?> GetAsync(Guid id)
    {
        const string sql = """
                           SELECT * FROM "Days"
                           WHERE "Id"=@Id
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<Day>(sql, new { Id = id });
    }

    public async Task<List<Day>> GetListByScheduleIdAsync(Guid scheduleId)
    {
        const string sql = """
                           SELECT * FROM "Days"
                           WHERE "ScheduleId"=@ScheduleId
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<Day>(sql, new { ScheduleId = scheduleId })).ToList();
    }
}