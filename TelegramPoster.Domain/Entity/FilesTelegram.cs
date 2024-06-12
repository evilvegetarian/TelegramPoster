using System.ComponentModel.DataAnnotations;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Domain.Entity;

public class FilesTelegram
{
    public required Guid Id { get; set; }
    public required Guid MessageTelegramId { get; set; }
    /// <summary>
    ///     Id файла в телеграме
    /// </summary>
    public required string TgFileId { get; set; }
    [MaxLength(1024)]
    public string? Caption { get; set; }
    public required ContentTypes Type { get; set; }
}