using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Models.TelegramBot;

namespace TelegramPoster.Application.Validator.Channel;

public interface IChannelValidator
{
    Task<AddChannelValidateResult> AddChannelValidate(ChannelForm channelForm, ModelStateDictionary modelState);
}