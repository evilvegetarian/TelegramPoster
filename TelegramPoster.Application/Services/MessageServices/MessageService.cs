using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Message;
using TelegramPoster.Application.Services.Shared;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Services.MessageServices;

public class MessageService : IMessageService
{
    private readonly IDayRepository dayRepository;
    private readonly IFilesTelegramRepository filesTelegramRepository;
    private readonly IGuidManager guidManager;
    private readonly IMessageTelegramRepository messageTelegramRepository;
    private readonly ITimePostingRepository postingRepository;
    private readonly IScheduleRepository scheduleRepository;
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly ICryptoAES cryptoAES;

    public MessageService(ITimePostingRepository postingRepository,
        IScheduleRepository scheduleRepository,
        IGuidManager guidManager,
        IDayRepository dayRepository,
        IMessageTelegramRepository messageTelegramRepository,
        IFilesTelegramRepository filesTelegramRepository,
        ITelegramBotRepository telegramBotRepository,
        ICryptoAES cryptoAES)
    {
        this.messageTelegramRepository = messageTelegramRepository;
        this.filesTelegramRepository = filesTelegramRepository;
        this.postingRepository = postingRepository;
        this.scheduleRepository = scheduleRepository;
        this.guidManager = guidManager;
        this.dayRepository = dayRepository;
        this.telegramBotRepository = telegramBotRepository;
        this.cryptoAES = cryptoAES;
    }
    /// <summary>
    ///     Метод принимает сколько угодно медиафайлов и делает из них по одному сообщению
    /// </summary>
    public async Task GenerateOneMessagesFromFiles(MessagesFromFilesForm messagesFromFilesForm)
    {
        var bot = await telegramBotRepository.GetAsync(messagesFromFilesForm.BotId);

        var botClient = new TelegramBotClient(cryptoAES.Decrypt(bot.ApiTelegram));

        List<FileMessage> file = await FileSharedService.GetFileMessageInTelegramByFile(botClient, messagesFromFilesForm.Files, bot.ChatIdWithBotUser!.Value);

        var schedule = await scheduleRepository.GetAsync(messagesFromFilesForm.ScheduleId)
            ?? throw new BadHttpRequestException("");

        var days = await dayRepository.GetListByScheduleIdAsync(messagesFromFilesForm.ScheduleId);
        var timing = await postingRepository.GetByDayIdsAsync(days.ConvertAll(x => x.Id));
        Dictionary<DayOfWeek, List<TimeOnly>> dayTimeDict = [];
        foreach (var day in days)
        {
            var times = timing
                .Where(x => x.DayId == day.Id)
                .Select(x => x.Time)
                .OrderBy(x => x)
                .ToList();
            dayTimeDict.Add(day.DayOfWeek!.Value, times);
        }

        var allTimingMessages = await messageTelegramRepository.GetAllTimingMessageByScheduleIdAsync(messagesFromFilesForm.ScheduleId);

        var timesPosting = TimePostingSharedService.GetTimeForPosting(file.Count, dayTimeDict, allTimingMessages);
        var telegramMessages = new List<MessageTelegram>();
        var filesTelegram = new List<FilesTelegram>();
        for (int i = 0; i < file.Count; i++)
        {
            var messageTelegram = new MessageTelegram
            {
                Id = guidManager.NewGuid(),
                ScheduleId = messagesFromFilesForm.ScheduleId,
                Status = MessageStatus.Register,
                TimePosting = timesPosting[i],
                IsTextMessage = false,
            };

            telegramMessages.Add(messageTelegram);
            filesTelegram.Add(new FilesTelegram
            {
                Id = guidManager.NewGuid(),
                MessageTelegramId = messageTelegram.Id,
                TgFileId = file[i].Id,
                Type = file[i].Type,
            });
        }
        await messageTelegramRepository.AddListAsync(telegramMessages);
        await filesTelegramRepository.AddListAsync(filesTelegram);
    }
}

public record FileMessage(string Id,
                          ContentTypes Type);

public static class ContentTypeExtensions
{
    public static ContentTypes GetContentType(this IFormFile file)
    {
        if (file.ContentType.StartsWith("image"))
            return ContentTypes.Photo;
        else if (file.ContentType.StartsWith("video"))
            return ContentTypes.Video;
        return ContentTypes.NoOne;
    }
}