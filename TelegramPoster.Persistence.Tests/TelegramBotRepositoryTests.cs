using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Persistence.Tests;

public class TelegramBotRepositoryTests : IClassFixture<PersistenceTestFixture>
{
    private readonly EntityFactory factory;
    private readonly ITelegramBotRepository telegramBotRepository;

    public TelegramBotRepositoryTests(PersistenceTestFixture fixture)
    {
        telegramBotRepository = fixture.ServiceProvider.GetRequiredService<ITelegramBotRepository>();
        factory = new EntityFactory(
            fixture.ServiceProvider.GetRequiredService<IUserRepository>(),
            fixture.ServiceProvider.GetRequiredService<ITelegramBotRepository>(),
            fixture.ServiceProvider.GetRequiredService<IScheduleRepository>(),
            fixture.ServiceProvider.GetRequiredService<IDayRepository>(),
            fixture.ServiceProvider.GetRequiredService<ITimePostingRepository>()
        );
    }
    [Fact]
    public async Task GetTelegramBotMatchingTelegramInDb_ShouldReturnNull()
    {
        var telegramBotId = Guid.Parse("3a479a7f-b6d4-46c9-b570-a399e51d7a2a");
        var actual = await telegramBotRepository.GetAsync(telegramBotId);
        actual.Should().BeNull();
    }

    [Fact]
    public async Task AddDayAndGetByDayId_ShouldWorkCorrectly()
    {
        var user = await factory.CreateUserAsync("user1");
        var telegramBot = new TelegramBot
        {
            Id = Guid.Parse("4e609da3-13c4-48db-88f4-16d6e2371239"),
            NameBot = "новый бот",
            UserId = user.Id,
            BotStatus = BotStatus.Register,
            ApiTelegram = factory.GetRandomApiTelegram(),
            ChatIdWithBotUser=1213524524
        };

        await telegramBotRepository.AddAsync(telegramBot);
        var result = await telegramBotRepository.GetAsync(telegramBot.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(telegramBot);
    }

}
