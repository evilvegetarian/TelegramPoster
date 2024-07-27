using Hangfire;
using Telegram.Bot;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Background;

public class MessageService
{
    private readonly IMessageTelegramRepository messageTelegramRepository;
    private readonly ICryptoAES cryptoAES;

    public MessageService(IMessageTelegramRepository messageTelegramRepository, ICryptoAES cryptoAES)
    {
        this.messageTelegramRepository = messageTelegramRepository;
        this.cryptoAES = cryptoAES;
    }

    public async Task ProcessMessages()
    {
        var messages = await messageTelegramRepository.GetByStatusWithFileAndScheduleAndBotAsync(MessageStatus.Register);

        foreach (var item in messages)
        {
            BackgroundJob.Schedule(() => SendMessageAsync(item), item.TimePosting);
        }
        await messageTelegramRepository.UpdateStatusAsync(messages.Select(x => x.Id).ToList(), MessageStatus.InHandle);
    }

    public async Task SendMessageAsync(MessageTelegram messageTelegram)
    {
        var telegramBot = new TelegramBotClient(cryptoAES.Decrypt(messageTelegram.Schedule!.TelegramBot!.ApiTelegram));

        if (messageTelegram.FilesTelegrams.Any())
        {
            var media = messageTelegram.GetFiles();
            if (media?.Any() == true)
            {
                await telegramBot.SendMediaGroupAsync(messageTelegram.Schedule!.ChannelId, media: media!);
            }
        }
        else if (messageTelegram.TextMessage != null)
        {
            await telegramBot.SendTextMessageAsync(messageTelegram.Schedule!.ChannelId, messageTelegram.TextMessage);
        }

        await messageTelegramRepository.UpdateStatusAsync(messageTelegram.Id, MessageStatus.Send);
    }
}