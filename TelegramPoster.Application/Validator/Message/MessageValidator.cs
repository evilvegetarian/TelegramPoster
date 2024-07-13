using Microsoft.AspNetCore.Mvc.ModelBinding;
using Telegram.Bot;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Message;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Validator.Message;

public class MessageValidator : IMessageValidator
{
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IScheduleRepository scheduleRepository;
    private readonly ICryptoAES cryptoAES;

    public MessageValidator(IScheduleRepository scheduleRepository, ITelegramBotRepository telegramBotRepository, ICryptoAES cryptoAES, ICurrentUserProvider currentUserProvider)
    {
        this.telegramBotRepository = telegramBotRepository;
        this.currentUserProvider = currentUserProvider;
        this.scheduleRepository = scheduleRepository;
        this.cryptoAES = cryptoAES;
    }

    public async Task<MessagesFromFilesFormResultValidate> MessagesFromFilesFormValidator(MessagesFromFilesRequestForm messagesFromFilesForm, ModelStateDictionary modelState)
    {
        var result = new MessagesFromFilesFormResultValidate();
        var schedule = await scheduleRepository.GetAsync(messagesFromFilesForm.ScheduleId);

        if (schedule.AssertFound(modelState))
        {
            var bot = await telegramBotRepository.GetAsync(schedule!.BotId);

            if (bot.AssertFound(modelState))
            {
                var botClient = new TelegramBotClient(cryptoAES.Decrypt(bot!.ApiTelegram));

                if (bot.UserId != currentUserProvider.Current().UserId)
                {
                    modelState.AddModelError(nameof(User), "У пользователя нет доступа для данного бота");
                }
                if (botClient.AssertFound(modelState))
                {
                    if (bot.ChatIdWithBotUser != default)
                    {
                        var chat = await botClient.GetChatAsync(bot.ChatIdWithBotUser);
                        result.ChatIdWithBotUser = bot.ChatIdWithBotUser;
                    }
                    else
                    {
                        modelState.AddModelError(nameof(bot.ChatIdWithBotUser), "Нужно ввести чат с ботом");
                    }
                    result.TelegramBot = botClient;
                }
            }
        }

        return result;
    }
}

public class MessagesFromFilesFormResultValidate
{
    public TelegramBotClient? TelegramBot { get; set; }

    public long ChatIdWithBotUser { get; set; }
}
