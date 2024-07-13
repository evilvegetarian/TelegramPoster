using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.ScheduleTiming;

namespace TelegramPoster.Application.Validator.Day;

public class ScheduleTimingValidator(IScheduleRepository scheduleRepository) : IScheduleTimingValidator
{
    public async Task<bool> DayOfWeekScheduleFormValidate(ScheduleTimingDayOfWeekRequestForm dayOfWeekScheduleForm, ModelStateDictionary modelState)
    {
        var schedule = await scheduleRepository.GetAsync(dayOfWeekScheduleForm.ScheduleId);
        if (schedule == null)
        {
            modelState.AddModelError(nameof(schedule), "Расписания с текущим id не существует!");
        }
        foreach (var dayOfWeekForm in dayOfWeekScheduleForm.DayOfWeekForms)
        {
            if (dayOfWeekForm.StartPosting > dayOfWeekForm.EndPosting)
            {
                modelState.AddModelError(nameof(dayOfWeekScheduleForm.DayOfWeekForms), "Начальная дата должна быть меньше, чем конченная");
            }
            else if (TimeSpan.FromMinutes(dayOfWeekForm.Interval) > (dayOfWeekForm.EndPosting - dayOfWeekForm.StartPosting))
            {
                modelState.AddModelError(nameof(dayOfWeekForm.Interval), "Интервал не должен быть меньше ");
            }
        }
        return modelState.IsValid;
    }
}