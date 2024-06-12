using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Interfaces.Repositories;

public interface IMessageTelegramRepository
{
    Task AddAsync(MessageTelegram message);
    Task AddListAsync(List<MessageTelegram> messages);
    Task<List<DateTime>> GetAllTimingMessageByScheduleIdAsync(Guid scheduleId, bool? isPosted = null, bool currentDate = true);
    Task<MessageTelegram?> GetAsync(Guid id);
    Task<List<MessageTelegram>> GetByScheduleIdAsync(Guid scheduleId, bool? isPosted = null);
}