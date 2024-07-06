using Telegram.Bot.Types;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Background;

public static class FileExtensions
{
    public static IEnumerable<IAlbumInputMedia> GetFiles(this MessageTelegram messageTelegram)
    {
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

        return medias;
    }
}