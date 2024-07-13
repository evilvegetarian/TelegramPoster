using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TelegramPoster.Application.Models.Message;

public record MessagesFromFilesRequestForm
{
    [Required]
    public required List<IFormFile> Files { get; set; }

    [Required]
    public required Guid ScheduleId { get; set; }
}