namespace TelegramPoster.Application.Models;

public class RefreshRequestForm
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}

public class RefreshResponseModel:LoginResponseModel
{

}

public class LoginResponseModel
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}