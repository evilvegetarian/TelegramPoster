using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.Day;
using TelegramPoster.Application.Services.DayServices;
using TelegramPoster.Application.Validator.Day;

namespace TelegramPoster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DayScheduleController(IDayService dayService, IDayValidator dayValidator) : ControllerBase
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
    [HttpPost(nameof(DayOfWeekSchedule))]
    public async Task<IActionResult> DayOfWeekSchedule([FromBody] DayOfWeekScheduleForm dayOfWeekSchedule)
    {
        var resultValidate = await dayValidator.DayOfWeekScheduleFormValidate(dayOfWeekSchedule, ModelState);
        if (resultValidate)
        {
            await dayService.CreateForDayOfWeek(dayOfWeekSchedule);
            return Ok();
        }
        return BadRequest(ModelState);
    }
}