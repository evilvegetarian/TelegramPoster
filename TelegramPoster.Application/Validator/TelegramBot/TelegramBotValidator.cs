using Microsoft.AspNetCore.Mvc.ModelBinding;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Validator.TelegramBot;

public class TelegramBotValidator : ITelegramBotValidator
{
    private readonly ITelegramBotRepository botRepository;
    private readonly ICryptoAES cryptoAES;

    public TelegramBotValidator(ITelegramBotRepository botRepository, ICryptoAES cryptoAES)
    {
        this.botRepository = botRepository;
        this.cryptoAES = cryptoAES;
    }

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

            var botInfo = await bot.GetMeAsync();
            resultValidate.NameBot = botInfo.Username;

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
    public string? NameBot { get; set; }
    public string ApiTelegram { get; set; }
    public bool IsValid { get; set; }
}


public static class TelegramChanelExtension
{
    public static string ConvertToTelegramHandle(this string name)
    {
        string prefix = "https://t.me/";

        if (name.StartsWith(prefix))
        {
            return string.Concat("@", name.AsSpan(prefix.Length));
        }
        else if (!name.StartsWith('@'))
        {
            return string.Concat("@", name.AsSpan(prefix.Length));
        }

        return name;
    }
}