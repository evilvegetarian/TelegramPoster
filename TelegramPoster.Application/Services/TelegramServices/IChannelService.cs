
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Validator.Channel;

namespace TelegramPoster.Application.Services.TelegramServices;

public interface IChannelService
{
    Task<List<ChannelsViewModel>> GetChannelsAsync(Guid botId);
    Task<List<ChannelsBotModel>> GetChannelsBotAsync();
    Task AddChannelAsync(AddChannelValidateResult result, ChannelForm channelForm);
}
