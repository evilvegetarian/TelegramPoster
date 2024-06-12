using System.Data;

namespace TelegramPoster.Persistence;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}