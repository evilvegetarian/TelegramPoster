using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Day;

namespace TelegramPoster.Application.Validator.Day;

public interface IDayValidator
{
    Task<bool> DayOfWeekScheduleFormValidate(DayOfWeekScheduleForm dayOfWeekScheduleForm, ModelStateDictionary modelState);
}

public class DayValidator(IScheduleRepository scheduleRepository) : IDayValidator
{
    public async Task<bool> DayOfWeekScheduleFormValidate(DayOfWeekScheduleForm dayOfWeekScheduleForm, ModelStateDictionary modelState)
    {
        var schedule = await scheduleRepository.GetAsync(dayOfWeekScheduleForm.ScheduleId);
        if (schedule == null)
        {
            modelState.AddModelError(nameof(schedule), "Расписания с текущим id не существует!");
        }
        foreach (var dayOfWeekForm in dayOfWeekScheduleForm.DayOfWeekForms)
        {
            if (dayOfWeekForm.IsInterval)
            {
                if (dayOfWeekForm.CreateDayScheduleIntervalForms == null)
                {
                    modelState.AddModelError(nameof(dayOfWeekForm.CreateDayScheduleIntervalForms), "");
                }
                else
                {
                    var intervalForm = dayOfWeekForm.CreateDayScheduleIntervalForms;
                    if (intervalForm.StartPosting > intervalForm.EndPosting)
                    {
                        modelState.AddModelError(nameof(DayOfWeekScheduleIntervalForm), "Начальная дата должна быть меньше, чем конченная");
                    }
                    else if (TimeSpan.FromMinutes(intervalForm.Interval) > (intervalForm.EndPosting - intervalForm.StartPosting))
                    {
                        modelState.AddModelError(nameof(intervalForm.Interval), "");
                    }
                }
            }
            else
            {
                if (!dayOfWeekForm.TimesPosting.Any())
                {
                    modelState.AddModelError(nameof(dayOfWeekForm.TimesPosting), "");
                }
            }
        }

        return modelState.IsValid;
    }
}