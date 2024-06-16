using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.Message;
using TelegramPoster.Application.Services.MessageServices;
using TelegramPoster.Application.Validator.Message;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService messageServices;
    private readonly IMessageValidator messageValidator;

    public MessageController(IMessageService messageServices, IMessageValidator messageValidator)
    {
        this.messageServices = messageServices;
        this.messageValidator = messageValidator;
    }

    [RequestSizeLimit(int.MaxValue)]
    [HttpPost(nameof(OneMessagesFromFiles))]
    public async Task<IActionResult> OneMessagesFromFiles(MessagesFromFilesForm messagesFromFilesForm)
    {
        var resultValidate = await messageValidator.MessagesFromFilesFormValidator(messagesFromFilesForm, ModelState);
        if (resultValidate.IsValid)
        {
            await messageServices.GenerateOneMessagesFromFiles(messagesFromFilesForm, resultValidate);
            return Ok();
        }
        return BadRequest(ModelState);
    }
}