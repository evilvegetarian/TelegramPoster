using Microsoft.AspNetCore.Http;
using TelegramPoster.Auth.Interface;

namespace TelegramPoster.Auth;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public UserProvider Current()
    {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirst(JwtClaimTypes.UserId)?.Value;

        return userId != null
            ? new UserProvider
            {
                UserId = Guid.Parse(userId)
            }
            : new UserProvider();
    }
}

public class UserProvider
{
    public Guid UserId { get; init; }
}