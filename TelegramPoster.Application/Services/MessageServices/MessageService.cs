using Microsoft.AspNetCore.Http;
using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Message;
using TelegramPoster.Application.Services.Shared;
using TelegramPoster.Application.Validator.Message;
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
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly ICryptoAES cryptoAES;

    public MessageService(ITimePostingRepository postingRepository,
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
        this.guidManager = guidManager;
        this.dayRepository = dayRepository;
        this.telegramBotRepository = telegramBotRepository;
        this.cryptoAES = cryptoAES;
    }

    /// <summary>
    ///     Метод принимает сколько угодно медиафайлов и делает из них по одному сообщению
    /// </summary>
    public async Task GenerateOneMessagesFromFiles(MessagesFromFilesRequestForm messagesFromFilesForm, MessagesFromFilesFormResultValidate resultValidate)
    {
        var botClient = resultValidate.TelegramBot!;

        List<FileMessage> file = await FileSharedService.GetFileMessageInTelegramByFile(botClient, messagesFromFilesForm.Files, resultValidate.ChatIdWithBotUser);

        var days = await dayRepository.GetListWithTimeByScheduleIdAsync(messagesFromFilesForm.ScheduleId);
        var dayTimeDict = days.ToDictionary(day => day.DayOfWeek.Value, time => time.TimePostings.Select(x => x.Time).ToList());

        var allTimingMessages = await messageTelegramRepository.GetAllTimingMessageByScheduleIdAsync(messagesFromFilesForm.ScheduleId);

        var timesPosting = TimePostingSharedService.GetTimeForPosting(file.Count, dayTimeDict, allTimingMessages);
        var telegramMessages = new List<MessageTelegram>();
        var filesTelegram = new List<FilesTelegram>();
        for (int i = 0; i < file.Count; i++)
        {
            var messageId = guidManager.NewGuid();

            telegramMessages.Add(new MessageTelegram
            {
                Id = messageId,
                ScheduleId = messagesFromFilesForm.ScheduleId,
                Status = MessageStatus.Register,
                TimePosting = timesPosting[i],
                IsTextMessage = false,
            });

            filesTelegram.Add(new FilesTelegram
            {
                Id = guidManager.NewGuid(),
                MessageTelegramId = messageId,
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