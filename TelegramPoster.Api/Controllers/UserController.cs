using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.Registration;
using TelegramPoster.Application.Services.UserServices;

namespace TelegramPoster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
    IUserService userService,
    IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{


    [HttpPost(nameof(Register))]
    public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
    {
        await userService.Register(registrationModel);
        return Ok();
    }

    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginForm loginForm)
    {
        var token = await userService.Login(loginForm);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        httpContextAccessor.HttpContext?.Response.Cookies.Append("cock-cookies", token, cookieOptions);
        return Ok();
    }
}