using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Services.TelegramServices;
using TelegramPoster.Application.Validator.TelegramBot;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly ITelegramService telegramService;
    private readonly ITelegramBotValidator telegramBotValidator;

    public TelegramController(ITelegramService telegramService, ITelegramBotValidator telegramBotValidator)
    {
        this.telegramService = telegramService;
        this.telegramBotValidator = telegramBotValidator;
    }

    [HttpPost(nameof(ApiTelegram))]
    public async Task<IActionResult> ApiTelegram(ApiTelegramForm apiTelegramModel)
    {
        var resultValidate = await telegramBotValidator.ApiTelegramValidate(apiTelegramModel, ModelState);
        if (ModelState.IsValid)
        {
            await telegramService.AddTelegramBotAsync(resultValidate);
            return Ok();
        }
        return BadRequest(ModelState);
    }

    /// <summary>
    /// Получение всех телеграмм ботов пользователя
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(TelegramBots))]
    public async Task<IActionResult> TelegramBots()
    {
        return Ok(await telegramService.GetTelegramBots());
    }

    [HttpGet(nameof(Channels))]
    public async Task<IActionResult> Channels(Guid botId)
    {
        return Ok(await telegramService.GetChannelsAsync(botId));
    }

    [HttpPost(nameof(Channel))]
    public async Task<IActionResult> Channel(ChannelForm channelForm)
    {
        var result = await telegramBotValidator.AddChannelValidate(channelForm, ModelState);
        if (ModelState.IsValid)
        {
            await telegramService.AddChannelAsync(result, channelForm);
            return Ok();
        }
        return BadRequest(ModelState);
    }
}