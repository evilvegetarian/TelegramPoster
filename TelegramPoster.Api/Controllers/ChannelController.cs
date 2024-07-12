using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Services.TelegramServices;
using TelegramPoster.Application.Validator.Channel;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChannelController : ControllerBase
{
    private readonly IChannelService channelService;
    private readonly IChannelValidator channelValidator;

    public ChannelController(IChannelService channelService, IChannelValidator channelValidator)
    {
        this.channelService = channelService;
        this.channelValidator = channelValidator;
    }

    [HttpGet(nameof(Channels))]
    [Produces(typeof(List<ChannelsViewModel>))]
    public async Task<IActionResult> Channels([Required] Guid botId)
    {
        return Ok(await channelService.GetChannelsAsync(botId));
    }

    [HttpGet(nameof(ChannelsBot))]
    [Produces(typeof(List<ChannelsBotModel>))]
    public async Task<IActionResult> ChannelsBot()
    {
        return Ok(await channelService.GetChannelsBotAsync());
    }

    [HttpPost(nameof(Channel))]
    public async Task<IActionResult> Channel(ChannelForm channelForm)
    {
        var result = await channelValidator.AddChannelValidate(channelForm, ModelState);
        if (ModelState.IsValid)
        {
            await channelService.AddChannelAsync(result, channelForm);
            return Ok();
        }
        return BadRequest(ModelState);
    }
}