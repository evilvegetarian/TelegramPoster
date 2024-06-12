using TelegramPoster.Application.Models.Message;

namespace TelegramPoster.Application.Services.MessageServices;

public interface IMessageService
{
    Task GenerateOneMessagesFromFiles(MessagesFromFilesForm messagesFromFilesForm);
}
