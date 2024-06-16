using Microsoft.AspNetCore.Mvc.ModelBinding;
using Telegram.Bot;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Message;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Validator.Message;

public interface IMessageValidator
{
    Task<MessagesFromFilesFormResultValidate> MessagesFromFilesFormValidator(MessagesFromFilesForm messagesFromFilesForm, ModelStateDictionary modelState);
}

public class MessageValidator : IMessageValidator
{
    private readonly IScheduleRepository scheduleRepository;
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly ICryptoAES cryptoAES;

    public MessageValidator(IScheduleRepository scheduleRepository, ITelegramBotRepository telegramBotRepository, ICryptoAES cryptoAES)
    {
        this.scheduleRepository = scheduleRepository;
        this.telegramBotRepository = telegramBotRepository;
        this.cryptoAES = cryptoAES;
    }

    public async Task<MessagesFromFilesFormResultValidate> MessagesFromFilesFormValidator(MessagesFromFilesForm messagesFromFilesForm, ModelStateDictionary modelState)
    {
        var result = new MessagesFromFilesFormResultValidate();
        var schedule = await scheduleRepository.GetAsync(messagesFromFilesForm.ScheduleId);
        schedule.AssertFound(modelState);

        var bot = await telegramBotRepository.GetAsync(messagesFromFilesForm.BotId);
        bot.AssertFound(modelState);

        if (bot != null)
        {
            var botClient = new TelegramBotClient(cryptoAES.Decrypt(bot.ApiTelegram));
            botClient.AssertFound(modelState);
            if (botClient != null)
            {
                if (bot.ChatIdWithBotUser != null)
                {
                    var chat = await botClient.GetChatAsync(bot.ChatIdWithBotUser);
                    chat.AssertFound(modelState);
                    result.ChatIdWithBotUser = bot.ChatIdWithBotUser;
                }
                else
                {
                    modelState.AddModelError(nameof(bot.ChatIdWithBotUser), "Нужно ввести чат с ботом");
                }
                result.TelegramBot = botClient;
            }
        }
        result.IsValid = modelState.IsValid;
        return result;
    }
}

public class MessagesFromFilesFormResultValidate
{
    public bool IsValid { get; set; }

    public TelegramBotClient? TelegramBot { get; set; }

    public long ChatIdWithBotUser { get; set; }
}
