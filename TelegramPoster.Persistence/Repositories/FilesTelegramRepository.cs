using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class FilesTelegramRepository(ISqlConnectionFactory connection) : IFilesTelegramRepository
{
    public async Task AddAsync(FilesTelegram filesTelegram)
    {
        const string sql = """
                           INSERT INTO "FilesTelegram" ("Id", "Type", "TgFileId", "Caption", "MessageTelegramId")
                           VALUES (@Id, @Type, @TgFileId, @Caption, @MessageTelegramId)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, filesTelegram);
    }

    public async Task AddListAsync(List<FilesTelegram> filesTelegrams)
    {
        const string sql = """
                           INSERT INTO "FilesTelegram" ("Id", "Type", "TgFileId", "Caption", "MessageTelegramId")
                           VALUES (@Id, @Type, @TgFileId, @Caption, @MessageTelegramId)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, filesTelegrams);
    }
}