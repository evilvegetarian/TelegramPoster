using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class MessageTelegramRepository(ISqlConnectionFactory connection) : IMessageTelegramRepository
{
    public async Task AddAsync(MessageTelegram message)
    {
        const string sql = """
                           INSERT INTO "MessageTelegram" ("Id", "TimePosting", "TextMessage", "IsTextMessage", "Status", "ScheduleId")
                           VALUES (@Id, @TimePosting, @TextMessage, @IsTextMessage, @Status, @ScheduleId)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, message);
    }

    public async Task AddListAsync(List<MessageTelegram> messages)
    {
        const string sql = """
                           INSERT INTO "MessageTelegram" ("Id", "TimePosting", "TextMessage", "IsTextMessage", "Status", "ScheduleId")
                           VALUES (@Id, @TimePosting, @TextMessage, @IsTextMessage, @Status, @ScheduleId)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, messages);
    }

    public async Task<MessageTelegram?> GetAsync(Guid id)
    {
        const string sql = """
                           SELECT * FROM "MessageTelegram"
                           WHERE "Id"=@Id
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<MessageTelegram>(sql, new { Id = id });
    }

    public async Task<List<MessageTelegram>> GetByScheduleIdAsync(Guid scheduleId, bool? isPosted = null)
    {
        var sql = """
                  SELECT * FROM "MessageTelegram"
                  WHERE "ScheduleId"=@ScheduleId
                  """;

        if (isPosted.HasValue)
        {
            sql += """
                   AND "IsPosted"=@IsPosted
                   """;
        }

        using var db = connection.Create();
        return (await db.QueryAsync<MessageTelegram>(sql, new { ScheduleId = scheduleId, IsPosted = isPosted })).ToList();
    }

    public async Task<List<DateTime>> GetAllTimingMessageByScheduleIdAsync(Guid scheduleId, bool? isPosted = null, bool currentDate = true)
    {
        var sql = """
                  SELECT "TimePosting"
                  FROM "MessageTelegram"
                  WHERE "ScheduleId"=@ScheduleId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("ScheduleId", scheduleId);

        if (isPosted.HasValue)
        {
            sql += """ AND "Status"=@Status """;
            parameters.Add("Status", isPosted);
        }

        if (currentDate)
        {
            sql += """ AND "TimePosting" > @CurrentDate """;
            parameters.Add("CurrentDate", DateTime.UtcNow);
        }

        using var db = connection.Create();
        return (await db.QueryAsync<DateTime>(sql, parameters)).ToList();
    }
}
