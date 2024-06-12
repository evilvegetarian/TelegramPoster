using Microsoft.AspNetCore.Http;

namespace TelegramPoster.Application.Models.Message;

public record MessagesFromFilesForm(List<IFormFile> Files, Guid ScheduleId, Guid BotId);