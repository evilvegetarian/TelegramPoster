using Microsoft.AspNetCore.Mvc.ModelBinding;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using TelegramPoster.Application.Models.TelegramBot;

namespace TelegramPoster.Application.Validator.TelegramBot;

public interface ITelegramBotValidator
{
    Task<ApiTelegramValidateResult> ApiTelegramValidate(ApiTelegramForm apiTelegramForm, ModelStateDictionary modelState);
}

public class TelegramBotValidator : ITelegramBotValidator
{
    public async Task<ApiTelegramValidateResult> ApiTelegramValidate(ApiTelegramForm apiTelegramForm, ModelStateDictionary modelState)
    {
        var resultValidate = new ApiTelegramValidateResult
        {
            ApiTelegram = apiTelegramForm.ApiTelegram,
        };
        try
        {
            var bot = new TelegramBotClient(apiTelegramForm.ApiTelegram);
            var updates = await bot.GetUpdatesAsync();
            var chatId = updates
                .Select(u => u.Message?.Chat.Id)
                .FirstOrDefault();

            resultValidate.ChatWithBotId = chatId;

            var dd = await bot.GetMeAsync();
            resultValidate.NameBot = dd.Username;

            if (chatId == null)
            {
                modelState.AddModelError(nameof(ApiTelegramForm.ApiTelegram), "Похоже что вы еще не запустили бота, нажмите старт или напишите, любое сообщение в боте");
            }
        }
        catch (ApiRequestException e)
        {
            modelState.AddModelError(nameof(ApiTelegramForm.ApiTelegram), $"Введенный api не работает, введите правильный api телеграмма. {e}");
        }

        resultValidate.IsValid = modelState.IsValid;
        return resultValidate;
    }
}

public class ApiTelegramValidateResult
{
    public long? ChatWithBotId { get; set; }
    public required string ApiTelegram { get; set; }
    public string? NameBot { get; set; }
    public bool IsValid { get; set; }
}