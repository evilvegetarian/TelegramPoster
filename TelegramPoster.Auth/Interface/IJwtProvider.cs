namespace TelegramPoster.Auth.Interface;

public interface IJwtProvider
{
    string GenerateToken(TokenServiceBuildTokenPayload tokenPayload);
}