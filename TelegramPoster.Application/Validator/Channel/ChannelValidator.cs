using Microsoft.AspNetCore.Mvc.ModelBinding;
using Telegram.Bot;
using TelegramPoster.Application.Models.TelegramBot;
using TelegramPoster.Application.Validator.TelegramBot;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Validator.Channel;

public class ChannelValidator : IChannelValidator
{
    private readonly ITelegramBotRepository botRepository;
    private readonly ICryptoAES cryptoAES;

    public ChannelValidator(ICryptoAES cryptoAES, ITelegramBotRepository botRepository)
    {
        this.cryptoAES = cryptoAES;
        this.botRepository = botRepository;
    }

    public async Task<AddChannelValidateResult> AddChannelValidate(ChannelForm channelForm, ModelStateDictionary modelState)
    {
        var channelResult = new AddChannelValidateResult();

        var bot = await botRepository.GetAsync(channelForm.BotId);

        if (bot.AssertFound(modelState))
        {
            var telegramBot = new TelegramBotClient(cryptoAES.Decrypt(bot!.ApiTelegram));
            var chat = await telegramBot.GetChatAsync(channelForm.Channel.ConvertToTelegramHandle());
            channelResult.ChannelId = chat.Id;
        }
        channelResult.IsValid = modelState.IsValid;
        return channelResult;
    }
}


public class AddChannelValidateResult
{
    public long ChannelId { get; set; }
    public bool IsValid { get; set; }
}