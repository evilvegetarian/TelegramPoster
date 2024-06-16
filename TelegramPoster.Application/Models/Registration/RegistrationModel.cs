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
}