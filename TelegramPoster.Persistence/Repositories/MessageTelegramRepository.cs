using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;

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
                           WHERE "Id" = @Id
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<MessageTelegram>(sql, new { Id = id });
    }

    public async Task UpdateStatusAsync(List<Guid> ids, MessageStatus status)
    {
        const string sql = """
                       UPDATE "MessageTelegram"
                       SET "Status" = @Status
                       WHERE "Id" = ANY(@Ids)
                       """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, new { Status = status, Ids = ids.ToArray() });
    }

    public async Task UpdateStatusAsync(Guid id, MessageStatus status)
    {
        const string sql = """
                       UPDATE "MessageTelegram"
                       SET "Status" = @Status
                       WHERE "Id" = @Id
                       """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, new { Status = status, Ids = id });
    }

    public async Task<List<MessageTelegram>> GetByScheduleIdAsync(Guid scheduleId, bool? isPosted = null)
    {
        var sql = """
                  SELECT * FROM "MessageTelegram"
                  WHERE "ScheduleId" = @ScheduleId
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
            sql += """ AND "Status" = @Status """;
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

    public async Task<List<MessageTelegram>> GetByStatusWithFileAndScheduleAndBotAsync(MessageStatus messageStatus)
    {
        var timeFrom = DateTime.Now.TimeOfDay;
        var timeTo = DateTime.Now.TimeOfDay.Add(new TimeSpan(0, 0, 1, 0));

        var sql = """
                 SELECT mt.*, ft.*, s.*, tb.*
                 FROM "MessageTelegram" mt
                 INNER JOIN "FilesTelegram" ft ON ft."MessageTelegramId" = mt."Id"
                 LEFT JOIN "Schedule" s ON s."Id" = mt."ScheduleId"
                 LEFT JOIN "TelegramBot" tb ON tb."Id" = s."BotId"
                 WHERE mt."Status" = @Status
                 AND mt."TimePosting"::time > @TimeFrom
                 AND mt."TimePosting"::time <= @TimeTo
                 """;
        using var db = connection.Create();

        var messageFileScheduleLookup = new Dictionary<Guid, MessageTelegram>();

        await db.QueryAsync<MessageTelegram, FilesTelegram, Schedule, TelegramBot, MessageTelegram>(
            sql,
            (message, file, schedule, bot) =>
            {
                if (!messageFileScheduleLookup.TryGetValue(message.Id, out var messageEntry))
                {
                    messageEntry = message;
                    messageEntry.Schedule = schedule;
                    messageFileScheduleLookup.Add(message.Id, messageEntry);
                }

                if (schedule != null)
                {
                    schedule.TelegramBot = bot;
                }

                if (file != null)
                {
                    messageEntry.FilesTelegrams!.Add(file);
                }

                return messageEntry;
            },
            new { Status = messageStatus, TimeFrom = timeFrom, TimeTo = timeTo },
            splitOn: "Id,Id,Id"
        );

        return messageFileScheduleLookup.Values.ToList();
    }
}