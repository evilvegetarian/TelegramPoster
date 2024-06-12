using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Day;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Services.DayServices;

public class DayService : IDayService
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IDayRepository dayRepository;
    private readonly IGuidManager guidManager;
    private readonly ITimePostingRepository timePostingRepository;
    private readonly ICryptoAES cryptoAES;

    public DayService(IGuidManager guidManager,
        IDayRepository dayRepository,
        ITimePostingRepository timePostingRepository,
        ICurrentUserProvider currentUserProvider,
        ICryptoAES cryptoAES)
    {
        this.guidManager = guidManager;
        this.dayRepository = dayRepository;
        this.timePostingRepository = timePostingRepository;
        this.currentUserProvider = currentUserProvider;
        this.cryptoAES = cryptoAES;
    }

    public async Task CreateForDayOfWeek(DayOfWeekScheduleForm createDayOfWeekSchedule)
    {
        var days = new List<Day>();
        var timePostings = new List<TimePosting>();

        foreach (var dayForm in createDayOfWeekSchedule.DayOfWeekForms)
        {
            var newDay = new Day
            {
                Id = guidManager.NewGuid(),
                DayOfWeek = dayForm.DayOfWeekPosting,
                ScheduleId = createDayOfWeekSchedule.ScheduleId
            };

            days.Add(newDay);

            if (dayForm.IsInterval)
            {
                var dayTime = dayForm.CreateDayScheduleIntervalForms;
                if (dayTime == null)
                    throw new ArgumentException("ss");
                var i = dayTime.StartPosting;
                while (i <= dayTime.EndPosting)
                {
                    timePostings.Add(new TimePosting
                    {
                        Id = guidManager.NewGuid(),
                        DayId = newDay.Id,
                        Time = i
                    });
                    i = i.AddMinutes(dayTime.Interval);
                }
            }
            else
            {
                foreach (var dayTime in dayForm.TimesPosting)
                    timePostings.Add(new TimePosting
                    {
                        Id = guidManager.NewGuid(),
                        DayId = newDay.Id,
                        Time = dayTime
                    });
            }
        }

        await dayRepository.AddListAsync(days);
        await timePostingRepository.AddListAsync(timePostings);
    }

    public List<DayOfWeekViewModel> GetAllDayOfWeek()
    {
        return Enum.GetValues(typeof(DayOfWeek))
            .Cast<DayOfWeek>()
            .Select(x => new DayOfWeekViewModel((int)x, x.ToString()))
            .ToList();
    }
}