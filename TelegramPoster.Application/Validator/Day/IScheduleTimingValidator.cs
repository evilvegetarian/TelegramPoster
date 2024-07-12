using Microsoft.AspNetCore.Mvc.ModelBinding;
using TelegramPoster.Application.Models.Day;

namespace TelegramPoster.Application.Validator.Day;

public interface IScheduleTimingValidator
{
    Task<bool> DayOfWeekScheduleFormValidate(ScheduleTimingDayOfWeekForm dayOfWeekScheduleForm, ModelStateDictionary modelState);
}
