using Hangfire;
using TelegramPoster.Application.Interfaces.Repositories;
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

        builder.Services.AddScoped<MessageService>();

        builder.Services.AddPersistence(builder.Configuration);

        var app = builder.Build();

        app.UseHangfireDashboard();
        app.UseHangfireServer();

        var serviceProvider = app.Services;
        RecurringJob.AddOrUpdate(
        "process-messages-job",
         () => serviceProvider.GetService<MessageService>().ProcessMessages(),
        "* * * * *");

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}
public class MessageService
{
    private readonly ILogger<MessageService> logger;
    private readonly IServiceProvider services;
    private readonly ITelegramBotService telegramBotService;

    public MessageService(ILogger<MessageService> logger, IServiceProvider services, ITelegramBotService telegramBotService)
    {
        this.logger = logger;
        this.services = services;
        this.telegramBotService = telegramBotService;
    }

    public async Task ProcessMessages()
    {
        using var scope = services.CreateScope();

        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMessageTelegramRepository>();

        var messages = await scopedProcessingService.GetByStatusWithFileAsync(MessageStatus.Register);

        foreach (var item in messages)
        {
            BackgroundJob.Schedule(() => telegramBotService.SendMessageAsync(item), item.TimePosting);
        }
    }
}

public class MessageSeervice
{
    private readonly IServiceProvider services;
    private readonly ILogger<MessageService> logger;


    public MessageSeervice(IServiceProvider services, ILogger<MessageService> logger)
    {
        this.services = services;
        this.logger = logger;
    }
}

public interface ITelegramBotService
{
    Task SendMessageAsync(MessageTelegram messageTelegram);
}

public class TelegramBotService : ITelegramBotService
{
    public async Task SendMessageAsync(MessageTelegram messageTelegram)
    {

    }
}
