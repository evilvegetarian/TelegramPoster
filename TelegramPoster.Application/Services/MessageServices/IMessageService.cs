using TelegramPoster.Application.Models.Message;
using TelegramPoster.Application.Validator.Message;

namespace TelegramPoster.Application.Services.MessageServices;

public interface IMessageService
{
    Task GenerateOneMessagesFromFiles(MessagesFromFilesRequestForm messagesFromFilesForm, MessagesFromFilesFormResultValidate resultValidate);
}
