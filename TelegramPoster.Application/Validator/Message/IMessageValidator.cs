using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Models.Message;

namespace TelegramPoster.Application.Validator.Message;

public interface IMessageValidator
{
    Task<MessagesFromFilesFormResultValidate> MessagesFromFilesFormValidator(MessagesFromFilesForm messagesFromFilesForm, ModelStateDictionary modelState);
}
