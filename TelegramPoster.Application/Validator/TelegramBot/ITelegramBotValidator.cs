using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Models.TelegramBot;

namespace TelegramPoster.Application.Validator.TelegramBot;

public interface ITelegramBotValidator
{
    Task<ApiTelegramValidateResult> ApiTelegramValidate(ApiTelegramForm apiTelegramForm, ModelStateDictionary modelState);
    Task<AddChannelValidateResult> AddChannelValidate(ChannelForm channelForm, ModelStateDictionary modelState);
}
