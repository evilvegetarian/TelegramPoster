using Telegram.Bot;
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Validator.Channel;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Services.TelegramServices;

public class ChannelService : IChannelService
{
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly ICryptoAES cryptoAES;
    private readonly IBotChannelRepository channelRepository;

    public ChannelService(ITelegramBotRepository telegramBotRepository, ICurrentUserProvider currentUserProvider, ICryptoAES cryptoAES, IBotChannelRepository channelRepository)
    {
        this.telegramBotRepository = telegramBotRepository;
        this.currentUserProvider = currentUserProvider;
        this.cryptoAES = cryptoAES;
        this.channelRepository = channelRepository;
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

    public async Task<List<ChannelsBotModel>> GetChannelsBotAsync()
    {
        var userId = currentUserProvider.Current().UserId;

        var allBotUser = await telegramBotRepository.GetByUserIdAsync(userId);
        var channels = await channelRepository.GetListByBotIdsAsync(allBotUser.Select(x => x.Id));

        return channels.Select(channel =>
        {
            var bot = allBotUser.Find(x => x.Id == channel.BotId);
            var telega = new TelegramBotClient(cryptoAES.Decrypt(bot.ApiTelegram));

            return new ChannelsBotModel
            {
                BotId = bot.Id,
                ChannelId = channel.ChannelId,
                NameChannel = telega.GetChatAsync(channel.ChannelId).Result.Title,
                NameBot = bot.NameBot
            };
        }).ToList();
    }
}


public class ChannelsBotModel
{
    public required Guid BotId { get; set; }

    public required string NameBot { get; set; }

    public required long ChannelId { get; init; }
    public required string NameChannel { get; init; }
}

public class ChannelsViewModel
{
    public required long ChannelId { get; init; }
    public required string NameChannel { get; init; }
}