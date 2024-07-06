using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TelegramPoster.Application.Models.Registration;

public class RegistrationModel
{
    [EmailAddress]
    public required string Email { get; init; }
    public required string UserName { get; init; }

    /// <summary>
    /// Минимум 8 символов
    /// Минимум одну заглавную букву
    /// Минимум одну строчную букву
    /// Минимум одну цифру
    /// Минимум один специальный символ
    /// </summary>
    [PasswordComplexity]
    public required string Password { get; init; }
    [Phone]
    public required string PhoneNumber { get; init; }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class PasswordComplexityAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("Необходим пароль.");

        var password = value.ToString();

        if (password!.Length < 8)
            return new ValidationResult("Пароль должен быть длиной не менее 8 символов.");

        if (!Regex.IsMatch(password, "[A-Z]"))
            return new ValidationResult("Пароль должен содержать хотя бы одну заглавную букву.");

        if (!Regex.IsMatch(password, "[a-z]"))
            return new ValidationResult("Пароль должен содержать хотя бы одну строчную букву.");

        if (!Regex.IsMatch(password, "[0-9]"))
            return new ValidationResult("Пароль должен содержать хотя бы одну цифру.");

        if (!Regex.IsMatch(password, @"[\W_]"))
            return new ValidationResult("Пароль должен содержать хотя бы один специальный символ.");

        return ValidationResult.Success;
    }
}