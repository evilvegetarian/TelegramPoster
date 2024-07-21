using System.Security.Claims;

namespace TelegramPoster.Auth.Interface;

public interface IJwtProvider
{
    string GenerateRefreshToken();
    string GenerateToken(TokenServiceBuildTokenPayload tokenPayload);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}