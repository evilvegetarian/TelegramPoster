using System.ComponentModel.DataAnnotations;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Domain.Entity;

public class MessageTelegram
{
    public required Guid Id { get; init; }
    public required Guid ScheduleId { get; init; }
    public required DateTime TimePosting { get; init; }
    [MaxLength(4096)]
    public string? TextMessage { get; init; }
    /// <summary>
    ///     Если сообщение текстовое в нем не могут быть файлы
    ///     Если в TextMessage больше 1024 символов, в нем не могут быть файлы,
    ///     Если меньше или равно 1024 то, текст становится caption к первому файлу
    /// </summary>
    public required bool IsTextMessage { get; init; }
    public required MessageStatus Status { get; init; }

    public List<FilesTelegram>? FilesTelegrams { get; set; } = [];
    public Schedule? Schedule { get; set; }
}