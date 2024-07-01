using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class DayRepository(ISqlConnectionFactory connection) : IDayRepository
{
    public async Task AddAsync(Day day)
    {
        const string sql = """
                           INSERT INTO "Day" ("Id", "ScheduleId", "DayOfWeek", "DateDay")
                           VALUES (@Id, @ScheduleId, @DayOfWeek, @DateDay)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, day);
    }

    public async Task AddListAsync(List<Day> days)
    {
        const string sql = """
                       INSERT INTO "Day" ("Id", "ScheduleId", "DayOfWeek", "DateDay")
                       VALUES (@Id, @ScheduleId, @DayOfWeek, @DateDay)
                       """;

        using var db = connection.Create();
        await db.ExecuteAsync(sql, days);
    }

    public async Task<Day?> GetAsync(Guid id)
    {
        const string sql = """
                           SELECT * FROM "Day"
                           WHERE "Id"=@Id
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<Day>(sql, new { Id = id });
    }

    public async Task<List<Day>> GetListByScheduleIdAsync(Guid scheduleId)
    {
        const string sql = """
                           SELECT * FROM "Day"
                           WHERE "ScheduleId"=@ScheduleId
                           """;
        using var db = connection.Create();
        return (await db.QueryAsync<Day>(sql, new { ScheduleId = scheduleId })).ToList();
    }

    public async Task<List<Day>> GetListWithTimeByScheduleIdAsync(Guid scheduleId)
    {
        const string sql = """
                           SELECT * 
                           FROM "Day" day
                           JOIN "TimePosting" tp on tp."DayId" =day."Id" 
                           WHERE day."ScheduleId"=@ScheduleId
                           """;
        using var db = connection.Create();

        var dayTimePostingsLookup = new Dictionary<Guid, Day>();

        await db.QueryAsync<Day, TimePosting, Day>(
            sql,
            (day, timePosting) =>
            {
                if (!dayTimePostingsLookup.TryGetValue(day.Id, out var dayEntry))
                {
                    dayEntry = day;
                    dayEntry.TimePostings = new List<TimePosting>();
                    dayTimePostingsLookup.Add(day.Id, dayEntry);
                }

                if (timePosting != null)
                {
                    dayEntry.TimePostings.Add(timePosting);
                }

                return dayEntry;
            },
            new { ScheduleId = scheduleId },
            splitOn: "Id"
        );

        var result = dayTimePostingsLookup.Values.ToList();
        return result;
    }
}