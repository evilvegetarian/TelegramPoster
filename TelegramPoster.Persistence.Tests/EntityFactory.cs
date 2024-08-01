using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Domain.Enum;
using TelegramPoster.Persistence.Repositories;
using User = TelegramPoster.Domain.Entity.User;

namespace TelegramPoster.Persistence.Tests;

public class EntityFactory
{
    private readonly IUserRepository userRepository;
    private readonly ITelegramBotRepository telegramBotRepository;
    private readonly IScheduleRepository scheduleRepository;
    private readonly IDayRepository dayRepository;
    private readonly ITimePostingRepository timePostingRepository;

    public EntityFactory(
        IUserRepository userRepository,
        ITelegramBotRepository telegramBotRepository,
        IScheduleRepository scheduleRepository,
        IDayRepository dayRepository,
        ITimePostingRepository timePostingRepository)
    {
        this.userRepository = userRepository;
        this.telegramBotRepository = telegramBotRepository;
        this.scheduleRepository = scheduleRepository;
        this.dayRepository = dayRepository;
        this.timePostingRepository = timePostingRepository;
    }

    public async Task<User> CreateUserAsync(string userName)
    {
        Random random = new Random();
        var phone = random.Next(1000000000, 1999999999);
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            PhoneNumber = "+" + phone,
            PasswordHash = "password",
            Email = $"{userName}@mail.com"
        };
        await userRepository.AddAsync(user);
        return user;
    }

    public async Task<TelegramBot> CreateBotAsync(User user, string botName = "defaultBot")
    {
        var bot = new TelegramBot
        {
            Id = Guid.NewGuid(),
            BotStatus = BotStatus.Register,
            NameBot = botName,
            UserId = user.Id,
            ApiTelegram = "api_key"
        };
        await telegramBotRepository.AddAsync(bot);
        return bot;
    }

    public async Task<Schedule> CreateScheduleAsync(User user, TelegramBot bot, string scheduleName = "defaultSchedule")
    {
        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = scheduleName,
            BotId = bot.Id,
            ChannelId = 123456789
        };
        await scheduleRepository.AddAsync(schedule);
        return schedule;
    }

    public async Task<Day> CreateDayAsync(Schedule schedule, DayOfWeek dayOfWeek)
    {
        var day = new Day
        {
            Id = Guid.NewGuid(),
            DayOfWeek = dayOfWeek,
            ScheduleId = schedule.Id
        };
        await dayRepository.AddAsync(day);
        return day;
    }


    public async Task<TimePosting> CreateTimePostingAsync(Day day, TimeOnly time)
    {
        var timePosting = new TimePosting
        {
            Id = Guid.NewGuid(),
            DayId = day.Id,
            Time = time
        };
        await timePostingRepository.AddAsync(timePosting);
        return timePosting;
    }
}
