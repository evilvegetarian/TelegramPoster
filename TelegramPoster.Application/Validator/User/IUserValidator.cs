using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Models.Registration;

namespace TelegramPoster.Application.Validator.User;
public interface IUserValidator
{
    Task RegisterValidate(RegistrationRequestModel registrationModel, ModelStateDictionary modelState);
}