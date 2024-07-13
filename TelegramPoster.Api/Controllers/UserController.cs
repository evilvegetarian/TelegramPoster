using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.Registration;
using TelegramPoster.Application.Services.UserServices;
using TelegramPoster.Application.Validator.User;

namespace TelegramPoster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
    IUserService userService,
    IHttpContextAccessor httpContextAccessor,
    IUserValidator userValidator)
    : ControllerBase
{
    [HttpPost(nameof(Register))]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestModel registrationModel)
    {
        await userValidator.RegisterValidate(registrationModel, ModelState);
        if (ModelState.IsValid)
        {
            await userService.Register(registrationModel);
            return Ok();
        }
        return BadRequest(ModelState);
    }

    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginRequestForm loginForm)
    {
        var token = await userService.Login(loginForm);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        httpContextAccessor.HttpContext?.Response.Cookies.Append("cock-cookies", token, cookieOptions);
        return Ok(token);
    }
}