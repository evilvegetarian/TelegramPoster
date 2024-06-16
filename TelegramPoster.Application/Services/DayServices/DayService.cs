using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Day;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Services.DayServices;

public class DayService : IDayService
{
    private readonly IDayRepository dayRepository;
    private readonly IGuidManager guidManager;
    private readonly ITimePostingRepository timePostingRepository;

    public DayService(IGuidManager guidManager,
        IDayRepository dayRepository,
        ITimePostingRepository timePostingRepository)
    {
        this.guidManager = guidManager;
        this.dayRepository = dayRepository;
        this.timePostingRepository = timePostingRepository;
    }

    public async Task CreateForDayOfWeek(DayOfWeekScheduleForm createDayOfWeekSchedule)
    {
        var days = new List<Day>();
        var timePostings = new List<TimePosting>();
        var daysSchedule = await dayRepository.GetListByScheduleIdAsync(createDayOfWeekSchedule.ScheduleId);
        var timing = await timePostingRepository.GetByDayIdsAsync(daysSchedule.Select(x => x.Id).ToList());

        foreach (var dayForm in createDayOfWeekSchedule.DayOfWeekForms)
        {
            var dayExist = daysSchedule.Find(x => x.DayOfWeek == dayForm.DayOfWeekPosting);

            var day = dayExist ?? new Day
            {
                Id = guidManager.NewGuid(),
                DayOfWeek = dayForm.DayOfWeekPosting,
                ScheduleId = createDayOfWeekSchedule.ScheduleId
            };

            if (dayExist == null)
            {
                days.Add(day);
            }

            if (dayForm.IsInterval)
            {
                var dayTime = dayForm.CreateDayScheduleIntervalForms!;

                var i = dayTime.StartPosting;
                while (i <= dayTime.EndPosting)
                {
                    if (!timing.Exists(t => t.Time == i))
                    {
                        timePostings.Add(new TimePosting
                        {
                            Id = guidManager.NewGuid(),
                            DayId = day.Id,
                            Time = i
                        });
                    }
                    i = i.AddMinutes(dayTime.Interval);
                }
            }
            else
            {
                foreach (var dayTime in dayForm.TimesPosting)
                {
                    if (!timing.Exists(t => t.Time == dayTime))
                    {
                        timePostings.Add(new TimePosting
                        {
                            Id = guidManager.NewGuid(),
                            DayId = day.Id,
                            Time = dayTime
                        });
                    }
                }
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