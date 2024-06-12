using System.ComponentModel.DataAnnotations;

namespace TelegramPoster.Application.Models.Registration;

public class RegistrationModel
{
    [EmailAddress]
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    [Phone]
    public required string PhoneNumber { get; init; }

    /// <summary>
    /// Телеграм канал предназначен для регистрации файлов в базе тг. 
    /// Для дальнейшего использования в сообщениях.
    /// Нужен только для вэб версии
    /// </summary>
    public string? TelegramSvalka { get; init; }
}