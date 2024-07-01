using Hangfire;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Auth;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;
using TelegramPoster.Persistence;

namespace TelegramPoster.Background;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage();
        });

        builder.Services.AddHangfireServer();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<MessageService>();
        builder.Services.AddScoped<ITelegramBotService, TelegramBotService>();

        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddApiAuthentication(builder.Configuration);

        var app = builder.Build();

        app.UseHangfireDashboard();

        var serviceProvider = app.Services;

        RecurringJob.AddOrUpdate(
        "process-messages-job",
            () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<MessageService>().ProcessMessages(),
        "* * * * *");

        app.MapGet("/", context =>
        {
            context.Response.Redirect("hangfire");
            return Task.CompletedTask;
        });

        app.Run();
    }
}
public class MessageService
{
    private readonly ITelegramBotService telegramBotService;
    private readonly IMessageTelegramRepository messageTelegramRepository;

    public MessageService(ITelegramBotService telegramBotService, IMessageTelegramRepository messageTelegramRepository)
    {
        this.telegramBotService = telegramBotService;
        this.messageTelegramRepository = messageTelegramRepository;
    }

    public async Task ProcessMessages()
    {
        var messages = await messageTelegramRepository.GetByStatusWithFileAndScheduleAndBotAsync(MessageStatus.Register);

        foreach (var item in messages)
        {
            BackgroundJob.Schedule(() => telegramBotService.SendMessageAsync(item), item.TimePosting);
        }
        await messageTelegramRepository.UpdateStatusAsync(messages.Select(x => x.Id).ToList(), MessageStatus.InHandle);
    }
}

public interface ITelegramBotService
{
    Task SendMessageAsync(MessageTelegram messageTelegram);
}

public class TelegramBotService : ITelegramBotService
{
    private readonly ICryptoAES cryptoAES;
    private readonly IMessageTelegramRepository messageTelegramRepository;

    public TelegramBotService(ICryptoAES cryptoAES, IServiceProvider services, IMessageTelegramRepository messageTelegramRepository)
    {
        this.cryptoAES = cryptoAES;
        this.messageTelegramRepository = messageTelegramRepository;
    }

    public async Task SendMessageAsync(MessageTelegram messageTelegram)
    {
        var telegramBot = new TelegramBotClient(cryptoAES.Decrypt(messageTelegram.Schedule!.TelegramBot!.ApiTelegram));

        var medias = messageTelegram.FilesTelegrams.Select(file =>
        {
            if (file.Type == ContentTypes.Photo)
            {
                return new InputMediaPhoto(InputFile.FromFileId(file.TgFileId)) as IAlbumInputMedia;
            }
            else if (file.Type == ContentTypes.Video)
            {
                return new InputMediaVideo(InputFile.FromFileId(file.TgFileId)) as IAlbumInputMedia;
            }
            return null;
        }).Where(media => media != null);

        await telegramBot.SendMediaGroupAsync(messageTelegram.Schedule!.ChannelId, media: medias);

        await messageTelegramRepository.UpdateStatusAsync(messageTelegram.Id, MessageStatus.Send);
    }
}