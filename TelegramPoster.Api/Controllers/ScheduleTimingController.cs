﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TelegramPoster.Application.Models.ScheduleTiming;
using TelegramPoster.Application.Services.ScheduleTimingServices;
using TelegramPoster.Application.Validator.Day;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleTimingController(IScheduleTimingService scheduleTimingService, IScheduleTimingValidator dayValidator) : ControllerBase
{
    /// <summary>
    /// Возвращаются дни недели
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(DayOfWeeks))]
    public IActionResult DayOfWeeks()
    {
        var model = scheduleTimingService.GetAllDayOfWeek();
        return Ok(model);
    }

    /// <summary>
    ///     Создается расписание на дни недели, через интервал
    /// </summary>
    /// <param name="dayOfWeekSchedule"></param>
    /// <returns></returns>
    [HttpPost(nameof(ScheduleTimingDayOfWeek))]
    public async Task<IActionResult> ScheduleTimingDayOfWeek([FromBody] ScheduleTimingDayOfWeekRequestForm dayOfWeekSchedule)
    {
        await dayValidator.DayOfWeekScheduleFormValidate(dayOfWeekSchedule, ModelState);
        if (ModelState.IsValid)
        {
            await scheduleTimingService.CreateForDayOfWeek(dayOfWeekSchedule);
            return Ok();
        }
        return BadRequest(ModelState);
    }

    [HttpGet(nameof(ScheduleTimingDayOfWeek))]
    [Produces(typeof(ScheduleTimingResponseModel))]
    public async Task<IActionResult> ScheduleTimingDayOfWeek([Required] Guid scheduleId)
    {
        var response = await scheduleTimingService.GetDayOfWeekTiming(scheduleId);
        return Ok(response);
    }
}