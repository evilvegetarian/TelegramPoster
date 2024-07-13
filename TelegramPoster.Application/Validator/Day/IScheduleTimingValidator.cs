using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Models.ScheduleTiming;

namespace TelegramPoster.Application.Validator.Day;

public interface IScheduleTimingValidator
{
    Task<bool> DayOfWeekScheduleFormValidate(ScheduleTimingDayOfWeekRequestForm dayOfWeekScheduleForm, ModelStateDictionary modelState);
}
