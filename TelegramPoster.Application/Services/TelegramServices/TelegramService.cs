﻿using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Validator.TelegramBot;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Application.Services.TelegramServices;
public class TelegramService : ITelegramService
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly IGuidManager guidManager;
    private readonly ICryptoAES cryptoAES;

    public TelegramService(ICurrentUserProvider currentUserProvider, IGuidManager guidManager, ITelegramBotRepository telegramBotRepository, ICryptoAES cryptoAES)
    {
        this.currentUserProvider = currentUserProvider;
        this.guidManager = guidManager;
        this.telegramBotRepository = telegramBotRepository;
        this.cryptoAES = cryptoAES;
    }

    public async Task AddTelegramBotAsync(ApiTelegramValidateResult apiTelegramModel)
    {
        var telegramBot = new TelegramBot
        {
            Id = guidManager.NewGuid(),
            UserId = currentUserProvider.Current().UserId,
            ApiTelegram = cryptoAES.Encrypt(apiTelegramModel.ApiTelegram),
            BotStatus = BotStatus.Register,
            ChatIdWithBotUser = apiTelegramModel.ChatWithBotId.Value,
        };

        await telegramBotRepository.AddAsync(telegramBot);
    }
}
