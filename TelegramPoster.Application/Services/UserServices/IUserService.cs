using TelegramPoster.Application.Models.Registration;

namespace TelegramPoster.Application.Services.UserServices;

public interface IUserService
{
    Task<string> Login(LoginRequestForm loginForm);
    Task Register(RegistrationRequestModel registrationModel);
}