using Npgsql;
using System.Data;

namespace TelegramPoster.Persistence.Context;

public class PostgreSqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    private readonly string connectionString = connectionString;

    public IDbConnection Create()
    {
        return new NpgsqlConnection(connectionString);
    }
}