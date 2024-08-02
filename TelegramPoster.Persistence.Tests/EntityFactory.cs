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
            Email = $"{userName}@mail.com",
            RefreshToken = "RefreshToken",
            RefreshTokenExpiryTime = new DateTime(2024, 05, 10, 21, 15, 11),
            TelegramUserName = userName,
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
            ApiTelegram = GetRandomApiTelegram(),
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

    public Dictionary<string, string> ApiTelegramExample()
    {
        return new Dictionary<string, string>
        {
            { "1213232159:AFanfvdjrenfdv_MJsjesk345jfdsksDRgS", "JIOsTsZ1cyCzaEcJfKRLo5JN59JiZNclZN1QIFz5iMclplyoArVhZ5s9zOsaUMO9" },
            { "7416326527:AAFqDZsQ1R11m1hKRxzfDO1CuXDF-UPlWVE", "FJCTwl9eT7V7ZYD/tSDzQaav2Fz51ox5uOARwqUX3xIHeW/qOL3LQUVU8isYqd+s" },
            { "5325623636:DDDqDZsQ1R11m1hKRxDDDO1CuXDF-SslWVE", "9jYUNLqOws1I03gpYC2Nmz6XKNZX6/2qcrDeXvlAfelYi8LSasCQTmNhTDM1eZb6" },
            { "7322994561:AAFS9VePCUG2p-Sp-5olhRPWtXJxt50Khls", "NGeRW2xU5z5iULTMGkDvZxwK1rj7Qs2eezaQpilSh4l+mIJ8XDVFgtnk29Wwevc/" },
            { "2342354363:ASS9VePDS2p-Sp-5olhRccPWtXXxt50DDls", "Z/Qy0dcJaMZ2+DFb+gYpa0WMo2J9ONAfSTDG9YT/6ydmImFnSw1uzBRCIgyo9QD1" }
        };
    }

    public string GetRandomApiTelegram()
    {
        var random = new Random();
        var apiDict = ApiTelegramExample();
        var rnd = random.Next(0, apiDict.Count - 1);
        var item = apiDict.ElementAt(rnd);
        return item.Value;
    }

}
