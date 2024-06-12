using Dapper;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;

public class UserRepository(ISqlConnectionFactory connection) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
                           SELECT * FROM "User"
                           WHERE "Email"=@Email
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        const string sql = """
                           SELECT * FROM "User"
                           WHERE "UserName"=@UserName
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<User>(sql, new { UserName = userName });
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        const string sql = """
                           SELECT * FROM "User"
                           WHERE "PhoneNumber"=@PhoneNumber
                           """;
        using var db = connection.Create();
        return await db.QueryFirstOrDefaultAsync<User>(sql, new { PhoneNumber = phone });
    }

    public async Task AddAsync(User user)
    {
        const string sql = """
                           INSERT INTO "User" ("Id", "UserName", "PasswordHash", "Email", "TelegramUserName", "PhoneNumber")
                           VALUES (@Id, @UserName, @PasswordHash, @Email, @TelegramUserName, @PhoneNumber)
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, new
        {
            user.Id,
            user.UserName,
            user.PasswordHash,
            user.Email,
            user.TelegramUserName,
            user.PhoneNumber
        });
    }

    public async Task UpdateAsync(User user)
    {
        const string sql = """
                           UPDATE "User"
                           SET "UserName"=@UserName,
                               "TelegramUserName"=@TelegramUserName,
                               "PhoneNumber"=@PhoneNumber
                           WHERE "Id"=@Id
                           """;
        using var db = connection.Create();
        await db.ExecuteAsync(sql, new
        {
            user.UserName,
            user.TelegramUserName,
            user.PhoneNumber,
            user.Id
        });
    }
}