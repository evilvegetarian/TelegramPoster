using TelegramPoster.Application.Interfaces;
using UUIDNext;

namespace Utility.GuidHelper;

public class SequentialGuidManager : IGuidManager
{
    public Guid NewGuid()
    {
        return Uuid.NewDatabaseFriendly(Database.PostgreSql);
    }
}