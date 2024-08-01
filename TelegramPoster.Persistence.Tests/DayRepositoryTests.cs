using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPoster.Persistence.Tests;

public class DayRepositoryTests : IClassFixture<PersistenceTestFixture>
{
    private readonly IDayRepository dayRepository;
    private readonly EntityFactory factory;

    public DayRepositoryTests(PersistenceTestFixture fixture)
    {
        dayRepository = fixture.ServiceProvider.GetRequiredService<IDayRepository>();
        factory = new EntityFactory(
            fixture.ServiceProvider.GetRequiredService<IUserRepository>(),
            fixture.ServiceProvider.GetRequiredService<ITelegramBotRepository>(),
            fixture.ServiceProvider.GetRequiredService<IScheduleRepository>(),
            fixture.ServiceProvider.GetRequiredService<IDayRepository>(),
            fixture.ServiceProvider.GetRequiredService<ITimePostingRepository>()
        );
    }

    [Fact]
    public async Task GetDayWhenNoMatchingDayIdInDb_ShouldReturnNull()
    {
        var dayId = Guid.Parse("cdbd7da1-2ebe-4d78-ba90-d32c21cc47e2");

        var actual = await dayRepository.GetAsync(dayId);
        actual.Should().BeNull();
    }

    [Fact]
    public async Task AddDayAndGetByDayId_ShouldWorkCorrectly()
    {
        var user = await factory.CreateUserAsync("user1");
        var bot = await factory.CreateBotAsync(user);
        var schedule = await factory.CreateScheduleAsync(user, bot);
        var day = await factory.CreateDayAsync(schedule, DayOfWeek.Saturday);

        var result = await dayRepository.GetAsync(day.Id);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(day);
    }

    [Fact]
    public async Task AddListDayAndGetByScheduleId_ShouldWorkCorrectly()
    {
        var user = await factory.CreateUserAsync("user2");
        var bot = await factory.CreateBotAsync(user);
        var schedule = await factory.CreateScheduleAsync(user, bot);

        var days = new List<Day>
        {
            new()
            {
                Id=Guid.Parse("26a94a20-b60a-4f86-add2-b7650b25d9be"),
                ScheduleId=schedule.Id,
                DayOfWeek=DayOfWeek.Saturday
            },
            new()
            {
                Id=Guid.Parse("4e098d9c-62f6-488e-905b-99844f769458"),
                ScheduleId = schedule.Id,
                DayOfWeek = DayOfWeek.Friday
            },
            new()
            {
                Id=Guid.Parse("f6f0a588-f3d2-42f7-8cbf-1b652cd49b15"),
                ScheduleId=schedule.Id,
                DayOfWeek=DayOfWeek.Monday
            }
        };
        await dayRepository.AddAsync(days);

        var result = await dayRepository.GetListByScheduleIdAsync(schedule.Id);
        result.Should().NotBeNullOrEmpty();
        result.Count.Should().Be(days.Count);
        result.Should().BeEquivalentTo(days);
    }

    [Fact]
    public async Task AddDaysWithTimingAndGetByScheduleIdDayWithTiming_ShouldWorkCorrectly()
    {
        var user = await factory.CreateUserAsync("user3");
        var bot = await factory.CreateBotAsync(user);
        var schedule = await factory.CreateScheduleAsync(user, bot);

        var day1 = new Day
        {
            Id = Guid.Parse("d9b3cbe1-801e-415a-b6cd-bcd0df46e682"),
            DayOfWeek = DayOfWeek.Saturday,
            ScheduleId = schedule.Id
        };

        var day2 = new Day
        {
            Id = Guid.Parse("839b4fb6-646e-4173-a4b5-eadf7060fcca"),
            DayOfWeek = DayOfWeek.Friday,
            ScheduleId = schedule.Id
        };

        await dayRepository.AddAsync([day1, day2]);

        var timing1 = await factory.CreateTimePostingAsync(day1, new TimeOnly(10, 15, 45));
        var timing2 = await factory.CreateTimePostingAsync(day1, new TimeOnly(23, 11, 23));
        var timing3 = await factory.CreateTimePostingAsync(day2, new TimeOnly(22, 45, 21));

        day1.TimePostings = [timing1, timing2];
        day2.TimePostings = [timing3];

        var result = await dayRepository.GetListWithTimeByScheduleIdAsync(schedule.Id);

        result.Should().ContainEquivalentOf(day1, options => options
            .Including(d => d.TimePostings));

        result.Should().ContainEquivalentOf(day2, options => options
            .Including(d => d.TimePostings));

        result.First(d => d.Id == day1.Id).TimePostings.Should().BeEquivalentTo([timing1, timing2]);
        result.First(d => d.Id == day2.Id).TimePostings.Should().BeEquivalentTo([timing3]);
    }

    [Fact]
    public async Task AddDaysWithDuplicateId_ShouldThrow()
    {
        var user = await factory.CreateUserAsync("user4");
        var bot = await factory.CreateBotAsync(user);
        var schedule = await factory.CreateScheduleAsync(user, bot);

        var days = new List<Day>()
        {
            new()
            {
                Id = Guid.Parse("2949ec08-8479-4075-9a85-3aef40ef1551"),
                DayOfWeek = DayOfWeek.Monday,
                ScheduleId = schedule.Id
            },
            new()
            {
                Id = Guid.Parse("2949ec08-8479-4075-9a85-3aef40ef1551"),
                DayOfWeek = DayOfWeek.Thursday,
                ScheduleId = schedule.Id
            },
            new()
            {
                Id = Guid.Parse("4cde4735-9a71-405d-8eb2-e1aed341f5c2"),
                DayOfWeek = DayOfWeek.Friday,
                ScheduleId = schedule.Id
            }
        };

        var act = async () => await dayRepository.AddAsync(days);

        await act.Should().ThrowAsync<PostgresException>();
    }
    [Fact]
    public async Task AddDayWithInvalidScheduleIdFk_ShouldThrowExceptionForInvalidData()
    {
        var day = new Day
        {
            Id = Guid.Parse("8743697b-3f19-44aa-add7-732dafc91a9b"),
            DayOfWeek = DayOfWeek.Thursday,
            ScheduleId = Guid.Parse("57763644-6cdd-40ea-bbec-f29c63471ff1")
        };

        var act = async () => await dayRepository.AddAsync(day);
        await act.Should().ThrowAsync<PostgresException>();
    }

    [Fact]
    public async Task GetListWithTimeByScheduleIdAsync_ShouldHandleMultipleSchedules()
    {
        var user = await factory.CreateUserAsync("user6");
        var bot = await factory.CreateBotAsync(user);
        var schedule1 = await factory.CreateScheduleAsync(user, bot);
        var schedule2 = await factory.CreateScheduleAsync(user, bot);

        var day1 = new Day
        {
            Id = Guid.Parse("40236e1b-472c-4129-953e-27fc88a653cd"),
            ScheduleId = schedule1.Id,
            DayOfWeek = DayOfWeek.Friday,
        };
        await dayRepository.AddAsync(day1);

        var timingForDay1 = await factory.CreateTimePostingAsync(day1, new TimeOnly(23, 11, 00));
        day1.TimePostings = [timingForDay1];

        var day2 = new Day
        {
            Id = Guid.Parse("d2a69e39-43c4-4454-b22b-b5c233f77a98"),
            ScheduleId = schedule2.Id,
            DayOfWeek = DayOfWeek.Saturday,
        };
        await dayRepository.AddAsync(day2);

        var timingForDay2 = await factory.CreateTimePostingAsync(day2, new TimeOnly(13, 21, 00));
        day2.TimePostings = [timingForDay2];

        var daysWithTimings1 = await dayRepository.GetListWithTimeByScheduleIdAsync(schedule1.Id);
        var daysWithTimings2 = await dayRepository.GetListWithTimeByScheduleIdAsync(schedule2.Id);

        daysWithTimings1.Should().ContainEquivalentOf(day1);
        daysWithTimings2.Should().ContainEquivalentOf(day2);
    }
}