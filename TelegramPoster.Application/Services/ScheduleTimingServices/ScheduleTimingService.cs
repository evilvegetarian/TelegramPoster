using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.ScheduleTiming;
using TelegramPoster.Application.Validator;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Services.ScheduleTimingServices;

public class ScheduleTimingService : IScheduleTimingService
{
    private readonly IDayRepository dayRepository;
    private readonly IGuidManager guidManager;
    private readonly ITimePostingRepository timePostingRepository;

    public ScheduleTimingService(IGuidManager guidManager,
        IDayRepository dayRepository,
        ITimePostingRepository timePostingRepository)
    {
        this.guidManager = guidManager;
        this.dayRepository = dayRepository;
        this.timePostingRepository = timePostingRepository;
    }

    public async Task CreateForDayOfWeek(ScheduleTimingDayOfWeekRequestForm createDayOfWeekSchedule)
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

            var i = dayForm.StartPosting;
            while (i <= dayForm.EndPosting)
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
                i = i.AddMinutes(dayForm.Interval);
            }
        }

        await dayRepository.AddListAsync(days);
        await timePostingRepository.AddListAsync(timePostings);
    }

    public List<DayOfWeekResponseModel> GetAllDayOfWeek()
    {
        return Enum.GetValues(typeof(DayOfWeek))
            .Cast<DayOfWeek>()
            .Select(x => new DayOfWeekResponseModel((int)x, x.ToString()))
            .ToList();
    }

    public async Task<ScheduleTimingResponseModel> GetDayOfWeekTiming(Guid scheduleId)
    {
        var timing = await dayRepository.GetListWithTimeByScheduleIdAsync(scheduleId);
        timing.AssertFound();

        var groupedTiming = timing.GroupBy(x => x.ScheduleId)
                                  .Select(g => new ScheduleTimingResponseModel
                                  {
                                      DayOfWeekModels = g.Select(day => new ScheduleTimingDayOfWeekResponseModel
                                      {
                                          DayOfWeekPosting = day.DayOfWeek!.Value,
                                          TimePosting = day.TimePostings.Select(x => x.Time).ToList(),
                                      }).ToList()

                                  }).FirstOrDefault();

        return groupedTiming ?? new ScheduleTimingResponseModel
        {
            DayOfWeekModels = new List<ScheduleTimingDayOfWeekResponseModel>()
        };
    }
}
