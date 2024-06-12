using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramPoster.Application.Services.MessageServices;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Application.Services.Shared;

public static class FileSharedService
{
    public static async Task<List<FileMessage>> GetFileMessageInTelegramByFile(TelegramBotClient botClient, List<IFormFile> files, long chatIdWithBotUser)
    {
        var chat = await botClient.GetChatAsync(chatIdWithBotUser);

        List<FileMessage> mediaMesses = [];
        foreach (var file in files)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var inputFile = new InputFileStream(memoryStream, file.FileName);
            Message message = null!;
            var type = file.GetContentType();
            string? fileId = null;
            switch (type)
            {
                case ContentTypes.Video:
                    message = await botClient.SendVideoAsync(chat.Id, inputFile);
                    fileId = message.Video?.FileId;
                    break;

                case ContentTypes.Photo:
                    message = await botClient.SendPhotoAsync(chat.Id, inputFile);
                    fileId = message.Photo?
                        .OrderByDescending(x => x.FileSize)
                        .Select(x => x.FileId)
                        .FirstOrDefault();
                    break;

                case ContentTypes.NoOne:
                    break;

                default:
                    throw new BadHttpRequestException("Тип данных какой то странный, изучи этот вопрос");
            }

            if (fileId != null)
                mediaMesses.Add(new FileMessage(fileId, type));

            await botClient.DeleteMessageAsync(chat.Id, message.MessageId);
        }
        return mediaMesses;
    }
}
