using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TelegramPoster.Auth.Interface;

namespace TelegramPoster.Auth;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions options = options.Value;

    public string GenerateToken(TokenServiceBuildTokenPayload tokenPayload)
    {
        Claim[] claims =
        [
            new Claim(JwtClaimTypes.UserId, tokenPayload.UserId.ToString())
        ];

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
            SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(options.ExpiresHours),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
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