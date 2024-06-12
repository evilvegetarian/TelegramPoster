using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Background;

public class BotManagerService : BackgroundService
{
    private readonly ILogger<BotManagerService> logger;
    private readonly ICryptoAES cryptoAES;
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly ConcurrentDictionary<string, ITelegramBotClient> bots = new();
    private readonly ConcurrentQueue<(string botToken, Update update)> updateQueue = new();
    private readonly SemaphoreSlim semaphore = new(100);

    public BotManagerService(ILogger<BotManagerService> logger, ITelegramBotRepository telegramBotRepository, ICurrentUserProvider currentUserProvider, ICryptoAES cryptoAES)
    {
        this.logger = logger;
        this.telegramBotRepository = telegramBotRepository;
        this.cryptoAES = cryptoAES;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var allTelegram = await telegramBotRepository.GetAsync();
        foreach (var telegram in allTelegram)
        {
            AddBot(cryptoAES.Decrypt(telegram.ApiTelegram));
        }

        _ = Task.Run(() => ProcessQueueAsync(stoppingToken), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void AddBot(string token)
    {
        var botClient = new TelegramBotClient(token);
        bots[token] = botClient;

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            async (client, update, cancellationToken) =>
            {
                updateQueue.Enqueue((token, update));
            },
            async (client, exception, cancellationToken) =>
            {
                logger.LogError($"Error in bot {token}: {exception.Message}");
            },
            receiverOptions
        );

        logger.LogInformation($"Bot with token {token} started.");
    }

    private async Task ProcessQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (updateQueue.TryDequeue(out var item))
            {
                await semaphore.WaitAsync(stoppingToken);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await HandleUpdateAsync(item.botToken, item.update, stoppingToken);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, stoppingToken);
            }

            await Task.Delay(100, stoppingToken); // Немного подождать перед следующим циклом
        }
    }

    private async Task HandleUpdateAsync(string botToken, Update update, CancellationToken stoppingToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var message = update.Message;
            logger.LogInformation($"Received a message from {message.Chat.Id} on bot {botToken}: {message.Text}");

            await bots[botToken].SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "You said:\n" + message.Text,
                cancellationToken: stoppingToken
            );
        }
    }
}