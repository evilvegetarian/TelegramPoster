using System.Security.Claims;

namespace TelegramPoster.Auth.Interface;

public interface IJwtProvider
{
    string GenerateRefreshToken();
    (string AccessToken, DateTime AcessExpireTime) GenerateToken(TokenServiceBuildTokenPayload tokenPayload);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}