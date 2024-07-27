using Telegram.Bot.Types;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Background;

public static class FileExtensions
{
    public static List<IAlbumInputMedia?> GetFiles(this MessageTelegram messageTelegram)
    {
        bool haveText = false;

        return messageTelegram.FilesTelegrams
            .Select(file =>
            {
                InputMedia media = file.Type switch
                {
                    ContentTypes.Photo => new InputMediaPhoto(InputFile.FromFileId(file.TgFileId)),
                    ContentTypes.Video => new InputMediaVideo(InputFile.FromFileId(file.TgFileId)),
                    _ => null
                };

                if (media != null && !haveText && messageTelegram.TextMessage != null)
                {
                    media.Caption = messageTelegram.TextMessage;

                    haveText = true;
                }
                return media;
            })
            .Where(media => media != null).Select(x=>x as IAlbumInputMedia).ToList();
    }
}
