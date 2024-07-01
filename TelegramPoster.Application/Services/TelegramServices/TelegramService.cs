using Telegram.Bot;
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
    private readonly IBotChannelRepository channelRepository;
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly IGuidManager guidManager;
    private readonly ICryptoAES cryptoAES;

    public TelegramService(
        ICurrentUserProvider currentUserProvider,
        IGuidManager guidManager,
        ITelegramBotRepository telegramBotRepository,
        ICryptoAES cryptoAES,
        IBotChannelRepository channelRepository)
    {
        this.currentUserProvider = currentUserProvider;
        this.guidManager = guidManager;
        this.telegramBotRepository = telegramBotRepository;
        this.cryptoAES = cryptoAES;
        this.channelRepository = channelRepository;
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

    public async Task<List<ChannelsViewModel>> GetChannelsAsync(Guid botId)
    {
        var bot = await telegramBotRepository.GetAsync(botId);

        var channels = await channelRepository.GetListByBotIdAsync(botId);
        var telega = new TelegramBotClient(cryptoAES.Decrypt(bot.ApiTelegram));

        return channels.Select(x => new ChannelsViewModel
        {
            ChannelId = x.ChannelId,
            NameChannel = telega.GetChatAsync(x.ChannelId).Result.Title
        }).ToList();
    }

    public async Task AddChannelAsync(AddChannelValidateResult result, ChannelForm channelForm)
    {
        var channelBot = new BotChannelLink
        {
            BotId = channelForm.BotId,
            ChannelId = result.ChannelId,
        };

        await channelRepository.AddAsync(channelBot);
    }
}


public class ChannelsViewModel
{
    public required long ChannelId { get; init; }
    public required string NameChannel { get; init; }
}