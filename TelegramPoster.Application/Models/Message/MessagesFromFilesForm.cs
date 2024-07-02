using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TelegramPoster.Application.Models.Message;

public record MessagesFromFilesForm
{
    [Required]
    public required List<IFormFile> Files { get; set; }

    [Required]
    public required Guid ScheduleId { get; set; }
}