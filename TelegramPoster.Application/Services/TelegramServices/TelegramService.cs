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
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IGuidManager guidManager;
    private readonly ICryptoAES cryptoAES;

    public TelegramService(
        ITelegramBotRepository telegramBotRepository,
        ICurrentUserProvider currentUserProvider,
        IGuidManager guidManager,
        ICryptoAES cryptoAES)
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

    public async Task<List<TelegramBotsResponseModel>> GetTelegramBots()
    {
        var userId = currentUserProvider.Current().UserId;

        var allBotUser = await telegramBotRepository.GetByUserIdAsync(userId);

        return allBotUser.Select(x => new TelegramBotsResponseModel
        {
            Id = x.Id,
            NameBot = x.NameBot,
        }).ToList();
    }
}