using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models;
using TelegramPoster.Application.Models.Registration;
using TelegramPoster.Application.Validator;
using TelegramPoster.Auth;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Services.UserServices;

public class UserService(
    IJwtProvider jwtProvider,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IGuidManager guidManager)
    : IUserService
{
    public async Task Register(RegistrationRequestForm registrationModel)
    {
        var resultGenerate = passwordHasher.Generate(registrationModel.Password);

        await userRepository.AddAsync(new User
        {
            Id = guidManager.NewGuid(),
            PasswordHash = resultGenerate,
            Email = registrationModel.Email,
            UserName = registrationModel.UserName,
            PhoneNumber = registrationModel.PhoneNumber
        });
    }

    public async Task<LoginResponseModel> Login(LoginRequestForm loginForm)
    {
        var user = await userRepository.CheckUserAsync(loginForm.UserNameOrEmail);
        user.AssertFound();

        var result = passwordHasher.CheckPassword(loginForm.Password, user!.PasswordHash);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        await userRepository.UpdateRefreshAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));
        var (newAccessToken, acessExpire) = jwtProvider.GenerateToken(new TokenServiceBuildTokenPayload { UserId = user.Id });
        return !result
            ? throw new InvalidOperationException("Failed to login")
            : new LoginResponseModel
            {
                AccessToken = newAccessToken,
                RefreshToken = refreshToken,
                AccessExpireTime = acessExpire,
            };
    }

    public async Task<RefreshResponseModel> RefreshToken(RefreshRequestForm form)
    {
        var principal = jwtProvider.GetPrincipalFromExpiredToken(form.AccessToken);
        var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.UserId)?.Value;

        var user = await userRepository.GetAsync(Guid.Parse(userId));

        if (user == null || user.RefreshToken != form.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        var (newAccessToken, accessExpire) = jwtProvider.GenerateToken(new TokenServiceBuildTokenPayload { UserId = user.Id });
        var newRefreshToken = jwtProvider.GenerateRefreshToken();

        await userRepository.UpdateRefreshAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(7));

        return new RefreshResponseModel
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessExpireTime = accessExpire
        };
    }
}