using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Persistence.Tests;

public class BotChannelRepositoryTests(PersistenceTestFixture fixture) : IClassFixture<PersistenceTestFixture>
{
    private readonly IBotChannelRepository botChannelRepository = fixture.ServiceProvider.GetRequiredService<IBotChannelRepository>();

    [Fact]
    public async Task AddBotChannelLinkAndGetListByBotIdAsync_ShouldWorkCorrectly()
    {
        var botId = Guid.Parse("2403f8ac-bb00-4b5d-bcb9-464d131b88bd");
        var channelId = 14124241412;
        var link = new BotChannelLink { BotId = botId, ChannelId = channelId };

        await botChannelRepository.AddAsync(link);

        var result = await botChannelRepository.GetListByBotIdAsync(botId);
        result.Should().NotBeNullOrEmpty();
        result.Should().ContainEquivalentOf(link);
    }

    [Fact]
    public async Task AddBotChannelLinksAndGetListByBotIdAsync_ShouldWorkCorrectly()
    {
        var links = new List<BotChannelLink>()
        {
            new() {BotId=Guid.Parse("176a06f9-5e46-403f-8bb8-a980ab16ecc4"),ChannelId=124142154536},
            new() {BotId=Guid.Parse("5f8afd04-c6cf-4e14-b6d9-9ed7f27e42a9"),ChannelId=213414513322},
            new() {BotId=Guid.Parse("5f8afd04-c6cf-4e14-b6d9-9ed7f27e42a9"),ChannelId=235463466472},
        };

        await botChannelRepository.AddAsync(links);

        var result = await botChannelRepository.GetListByBotIdsAsync(links.Select(x => x.BotId).ToList());
        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(links);
    }

    [Fact]
    public async Task AddBotChannelLinksTwo_ShouldThrowExceptionOnDuplicates()
    {
        var links = new List<BotChannelLink>()
        {
            new() { BotId = Guid.Parse("176a06f9-5e46-403f-8bb8-a980ab16ecc4"), ChannelId = 124142154536 },
            new() { BotId = Guid.Parse("5f8afd04-c6cf-4e14-b6d9-9ed7f27e42a9"), ChannelId = 213414513322 },
            new() { BotId = Guid.Parse("176a06f9-5e46-403f-8bb8-a980ab16ecc4"), ChannelId = 124142154536 }
        };

        var act = async () => await botChannelRepository.AddAsync(links);

        await act.Should().ThrowAsync<PostgresException>();
    }

    [Fact]
    public async Task GetBotChannelWhenNoMatchingBotIdInDb_ShouldReturnNull()
    {
        var botId = Guid.Parse("7da0eb8e-1b27-48ad-baf1-bd84c45870b5");

        var actual = await botChannelRepository.GetListByBotIdAsync(botId);
        actual.Should().BeNullOrEmpty();
    }
}