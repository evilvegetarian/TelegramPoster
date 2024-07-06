using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Registration;

namespace TelegramPoster.Application.Validator.User;
public class UserValidator : IUserValidator
{
    private readonly IUserRepository userRepository;

    public UserValidator(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task RegisterValidate(RegistrationModel registrationModel, ModelStateDictionary modelState)
    {
        var password = registrationModel.Password;

        if (string.IsNullOrWhiteSpace(password))
        {
            modelState.AddModelError("Password", "Необходим пароль.");
        }
        if (password.Length < 8)
        {
            modelState.AddModelError("Password", "Пароль должен быть длиной не менее 8 символов.");
        }

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigits = password.Any(char.IsDigit);
        bool hasSpecialChars = password.Any(ch => !char.IsLetterOrDigit(ch));

        if (!hasUpperCase)
        {
            modelState.AddModelError("Password", "Пароль должен содержать хотя бы одну заглавную букву.");
        }

        if (!hasLowerCase)
        {
            modelState.AddModelError("Password", "Пароль должен содержать хотя бы одну строчную букву.");
        }

        if (!hasDigits)
        {
            modelState.AddModelError("Password", "Пароль должен содержать хотя бы одну цифру.");
        }

        if (!hasSpecialChars)
        {
            modelState.AddModelError("Password", "Пароль должен содержать хотя бы один специальный символ.");
        }

        var user = await userRepository.GetByUserNameAsync(registrationModel.UserName);

        if (user != null)
        {
            modelState.AddModelError("UserName", "Такой username уже есть в базе");
        }
        var email = await userRepository.GetByEmailAsync(registrationModel.Email);

        if (email != null)
        {
            modelState.AddModelError("Email", "Такой email уже есть в базе");
        }

        //var phone = await userRepository.GetByPhoneAsync(registrationModel.PhoneNumber);
        //if (phone != null)
        //{
        //    modelState.AddModelError("Email", "Такой email уже есть в базе");
        //}
    }
}