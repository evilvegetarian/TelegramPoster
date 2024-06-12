using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Registration;
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
    public async Task Register(RegistrationModel registrationModel)
    {
        var resultGenerate = passwordHasher.Generate(registrationModel.Password);

        await userRepository.GetByEmailAsync(registrationModel.Email);
        await userRepository.GetByUserNameAsync(registrationModel.UserName);
        await userRepository.GetByPhoneAsync(registrationModel.PhoneNumber);

        await userRepository.AddAsync(new User
        {
            Id = guidManager.NewGuid(),
            PasswordHash = resultGenerate,
            Email = registrationModel.Email,
            UserName = registrationModel.UserName,
            PhoneNumber = registrationModel.PhoneNumber
        });
    }

    public async Task<string> Login(LoginForm loginForm)
    {
        var user = await userRepository.GetByUserNameAsync(loginForm.UserNameOrEmail);
        user ??= await userRepository.GetByEmailAsync(loginForm.UserNameOrEmail);
        if (user?.PasswordHash == null)
            throw new InvalidOperationException("Invalid user credentials.");
        var result = passwordHasher.CheckPassword(loginForm.Password, user.PasswordHash);

        if (!result)
            throw new InvalidOperationException("Failed to login");

        var token = jwtProvider.GenerateToken(new TokenServiceBuildTokenPayload { UserId = user.Id });
        return token;
    }
}