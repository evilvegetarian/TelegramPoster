using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TelegramPoster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiErrorController : ControllerBase
{
    [HttpGet]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (context == null)
        {
            return Problem(statusCode: 500, title: "Unknown error");
        }

        var exception = context.Error;

        if (exception is ArgumentNullException or ArgumentException)
        {
            return Problem(detail: exception.Message, statusCode: 400, title: "Bad Request");
        }
        else if (exception is UnauthorizedAccessException)
        {
            return Problem(detail: exception.Message, statusCode: 401, title: "Unauthorized");
        }
        else if (exception is KeyNotFoundException)
        {
            return Problem(detail: exception.Message, statusCode: 404, title: "Not Found");
        }

        return Problem(detail: exception.Message, statusCode: 500, title: "Internal Server Error");
    }
}