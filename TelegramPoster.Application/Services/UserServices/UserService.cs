using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
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
    public async Task Register(RegistrationRequestModel registrationModel)
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

    public async Task<string> Login(LoginRequestForm loginForm)
    {
        var user = await userRepository.CheckUserAsync(loginForm.UserNameOrEmail);
        user.AssertFound();

        var result = passwordHasher.CheckPassword(loginForm.Password, user!.PasswordHash);

        return !result
            ? throw new InvalidOperationException("Failed to login")
            : jwtProvider.GenerateToken(new TokenServiceBuildTokenPayload { UserId = user.Id });
    }
}