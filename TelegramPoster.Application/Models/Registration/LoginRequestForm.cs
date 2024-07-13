using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TelegramPoster.Application.Models.Registration;

public record LoginRequestForm
{
    [Required]
    [MinLength(4, ErrorMessage = "UserName must have more than 4 elements or equal 4")]
    public required string UserNameOrEmail { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Password must have more than 6 elements or equal 6")]
    public required string Password { get; set; }
    private bool BeAValidUsernameOrEmail(string username)
    {
        var emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        var isEmail = Regex.IsMatch(username, emailPattern);

        var usernamePattern = @"^[a-zA-Z0-9]*$";
        var isUsername = Regex.IsMatch(username, usernamePattern);

        return isEmail || isUsername;
    }
}