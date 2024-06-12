using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.Message;
using TelegramPoster.Application.Services.MessageServices;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService messageServices;

    public MessageController(IMessageService messageServices)
    {
        this.messageServices = messageServices;
    }

    [HttpPost(nameof(OneMessagesFromFiles))]
    public async Task<IActionResult> OneMessagesFromFiles(MessagesFromFilesForm messagesFromFilesForm)
    {
        await messageServices.GenerateOneMessagesFromFiles(messagesFromFilesForm);
        return Ok();
    }
}