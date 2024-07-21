using TelegramPoster.Application.Models;
using TelegramPoster.Application.Models.Registration;

namespace TelegramPoster.Application.Services.UserServices;

public interface IUserService
{
    Task<LoginResponseModel> Login(LoginRequestForm loginForm);
    Task<RefreshResponseModel> RefreshToken(RefreshRequestForm form);
    Task Register(RegistrationRequestModel registrationModel);
}