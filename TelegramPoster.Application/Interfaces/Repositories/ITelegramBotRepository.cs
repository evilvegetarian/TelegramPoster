﻿using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Repositories;
public interface ITelegramBotRepository
{
    Task AddAsync(TelegramBot telegramBot);
    Task<TelegramBot?> GetAsync(Guid id);
    Task<List<TelegramBot>> GetAsync();
    Task<List<TelegramBot>> GetByStatusAsync(BotStatus botStatus);
    Task<List<TelegramBot>> GetByUserIdAsync(Guid userId);
}