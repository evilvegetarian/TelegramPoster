using Telegram.Bot.Types;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Background;

public static class FileExtensions
{
    public static IEnumerable<IAlbumInputMedia?> GetFiles(this MessageTelegram messageTelegram)
    {
        bool haveText = false;

        return messageTelegram.FilesTelegrams
            .Select(file =>
            {
                IAlbumInputMedia media = file.Type switch
                {
                    ContentTypes.Photo => new InputMediaPhoto(InputFile.FromFileId(file.TgFileId)),
                    ContentTypes.Video => new InputMediaVideo(InputFile.FromFileId(file.TgFileId)),
                    _ => null
                };

                if (media != null && !haveText && messageTelegram.TextMessage != null)
                {
                    if (media is InputMediaPhoto photo)
                    {
                        photo.Caption = messageTelegram.TextMessage;
                    }
                    else if (media is InputMediaVideo video)
                    {
                        video.Caption = messageTelegram.TextMessage;
                    }

                    haveText = true;
                }
                return media;

            })
            .Where(media => media != null);
    }
}
