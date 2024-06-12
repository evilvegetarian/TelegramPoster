using TelegramPoster.Application.Models.Registration;

namespace TelegramPoster.Application.Services.UserServices;

public interface IUserService
{
    Task<string> Login(LoginForm loginForm);
    Task Register(RegistrationModel registrationModel);
}