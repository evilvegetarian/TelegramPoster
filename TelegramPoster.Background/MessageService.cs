using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Background;

public class MessageService : BackgroundService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IServiceProvider services;

    public MessageService(ILogger<MessageService> logger, IServiceProvider services)
    {
        _logger = logger;
        this.services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = services.CreateScope();

        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMessageTelegramRepository>();

        var dd = await scopedProcessingService.GetByStatusWithFileAsync(MessageStatus.Register);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
