using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Services.TelegramServices;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly ITelegramService telegramService;

    public TelegramController(ITelegramService telegramService)
    {
        this.telegramService = telegramService;
    }

    [HttpPost(nameof(ApiTelegram))]
    public async Task<IActionResult> ApiTelegram(ApiTelegramForm apiTelegramModel)
    {
        await telegramService.AddTelegramBotAsync(apiTelegramModel);
        return Ok();
    }
}