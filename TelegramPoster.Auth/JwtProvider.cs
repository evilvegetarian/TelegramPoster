using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TelegramPoster.Auth.Interface;

namespace TelegramPoster.Auth;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions options = options.Value;

    public (string AccessToken, DateTime AcessExpireTime) GenerateToken(TokenServiceBuildTokenPayload tokenPayload)
    {
        Claim[] claims =
        [
            new Claim(JwtClaimTypes.UserId, tokenPayload.UserId.ToString())
        ];

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var expireTime = DateTime.UtcNow.AddHours(options.ExpiresHours);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expireTime,
            signingCredentials: signingCredentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expireTime);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }


}

public class JwtOptions
{
    public required string SecretKey { get; set; }
    public string? Audience { get; set; }
    public string? Issue { get; set; }
    public int ExpiresHours { get; set; }
}

public struct JwtClaimTypes
{
    public const string Type = "type";
    public const string UserId = "userId";
}

public class TokenServiceBuildTokenPayload
{
    public Guid UserId { get; set; }
}