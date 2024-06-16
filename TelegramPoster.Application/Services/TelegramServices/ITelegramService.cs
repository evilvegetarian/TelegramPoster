
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Validator.TelegramBot;

namespace TelegramPoster.Application.Services.TelegramServices;

public interface ITelegramService
{
    Task AddTelegramBotAsync(ApiTelegramValidateResult apiTelegram);
}