
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Validator.TelegramBot;

namespace TelegramPoster.Application.Services.TelegramServices;

public interface ITelegramService
{
    Task AddChannelAsync(AddChannelValidateResult result, ChannelForm channelForm);
    Task AddTelegramBotAsync(ApiTelegramValidateResult apiTelegram);
    Task<List<ChannelsViewModel>> GetChannelsAsync(Guid botId);
    Task<List<TelegramBotsModelView>> GetTelegramBots();
}