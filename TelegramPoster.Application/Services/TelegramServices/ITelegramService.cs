
namespace TelegramPoster.Application.Services.TelegramServices;

public interface ITelegramService
{
    Task AddTelegramBotAsync(ApiTelegramForm apiTelegramModel);
}