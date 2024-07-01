using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;

namespace TelegramPoster.Application.Interfaces.Repositories;

public interface IMessageTelegramRepository
{
    Task AddAsync(MessageTelegram message);
    Task AddListAsync(List<MessageTelegram> messages);
    Task<List<DateTime>> GetAllTimingMessageByScheduleIdAsync(Guid scheduleId, bool? isPosted = null, bool currentDate = true);
    Task<MessageTelegram?> GetAsync(Guid id);
    Task<List<MessageTelegram>> GetByScheduleIdAsync(Guid scheduleId, bool? isPosted = null);
    Task<List<MessageTelegram>> GetByStatusWithFileAndScheduleAndBotAsync(MessageStatus messageStatus);
    Task UpdateStatusAsync(List<Guid> ids, MessageStatus status);
    Task UpdateStatusAsync(Guid id, MessageStatus status);
}