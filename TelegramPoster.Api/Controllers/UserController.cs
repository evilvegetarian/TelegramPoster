using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models;
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
    [Produces(typeof(LoginResponseModel))]
    public async Task<IActionResult> Login([FromBody] LoginRequestForm loginForm)
    {
        var login = await userService.Login(loginForm);
        AddCookie(login.AccessToken);
        return Ok(login);
    }


    [HttpPost(nameof(RefreshToken))]
    [Produces(typeof(RefreshResponseModel))]
    public async Task<IActionResult> RefreshToken(RefreshRequestForm form)
    {
        var refresh = await userService.RefreshToken(form);
        AddCookie(refresh.AccessToken);
        return Ok(refresh);
    }

    private void AddCookie(string accessToken)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        httpContextAccessor.HttpContext?.Response.Cookies.Append("cock-cookies", accessToken, cookieOptions);
    }
}