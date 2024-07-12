using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.Day;
using TelegramPoster.Application.Services.DayServices;
using TelegramPoster.Application.Validator.Day;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleTimingController(IDayService dayService, IScheduleTimingValidator dayValidator) : ControllerBase
{
    /// <summary>
    /// Возвращаются дни недели
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(DayOfWeeks))]
    public IActionResult DayOfWeeks()
    {
        var model = dayService.GetAllDayOfWeek();
        return Ok(model);
    }

    /// <summary>
    ///     Создается расписание на дни недели, через интервал
    /// </summary>
    /// <param name="dayOfWeekSchedule"></param>
    /// <returns></returns>
    [HttpPost(nameof(ScheduleTimingDayOfWeek))]
    public async Task<IActionResult> ScheduleTimingDayOfWeek([FromBody] ScheduleTimingDayOfWeekForm dayOfWeekSchedule)
    {
        await dayValidator.DayOfWeekScheduleFormValidate(dayOfWeekSchedule, ModelState);
        if (ModelState.IsValid)
        {
            await dayService.CreateForDayOfWeek(dayOfWeekSchedule);
            return Ok();
        }
        return BadRequest(ModelState);
    }
}