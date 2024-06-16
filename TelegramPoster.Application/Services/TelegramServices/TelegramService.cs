using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Validator.TelegramBot;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Services.TelegramServices;
public class TelegramService : ITelegramService
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly IGuidManager guidManager;
    private readonly ICryptoAES cryptoAES;

    public TelegramService(ICurrentUserProvider currentUserProvider, IGuidManager guidManager, ITelegramBotRepository telegramBotRepository, ICryptoAES cryptoAES)
    {
        this.currentUserProvider = currentUserProvider;
        this.guidManager = guidManager;
        this.telegramBotRepository = telegramBotRepository;
        this.cryptoAES = cryptoAES;
    }

    public async Task AddTelegramBotAsync(ApiTelegramValidateResult apiTelegramValidateResult)
    {
        var telegramBot = new TelegramBot
        {
            Id = guidManager.NewGuid(),
            UserId = currentUserProvider.Current().UserId,
            ApiTelegram = cryptoAES.Encrypt(apiTelegramValidateResult.ApiTelegram),
            BotStatus = BotStatus.Register,
            ChatIdWithBotUser = apiTelegramValidateResult.ChatWithBotId!.Value,
            NameBot = apiTelegramValidateResult.NameBot!
        };

        await telegramBotRepository.AddAsync(telegramBot);
    }

    public async Task<List<TelegramBotsModelView>> GetTelegramBots()
    {
        var userId = currentUserProvider.Current().UserId;

        var allBotUser = await telegramBotRepository.GetByUserIdAsync(userId);

        return allBotUser.Select(x => new TelegramBotsModelView
        {
            Id = x.Id,
            NameBot = x.NameBot,
        }).ToList();
    }
}
