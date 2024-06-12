using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Interfaces.Repositories;
public interface IFilesTelegramRepository
{
    Task AddAsync(FilesTelegram filesTelegram);
    Task AddListAsync(List<FilesTelegram> filesTelegrams);
}