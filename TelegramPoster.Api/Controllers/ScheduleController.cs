using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramPoster.Application.Models.Schedule;
using TelegramPoster.Application.Services.ScheduleServices;

namespace TelegramPoster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleController(IScheduleService scheduleService) : ControllerBase
{
    [HttpPost(nameof(Schedule))]
    public async Task<IActionResult> Schedule([FromBody] ScheduleDto scheduleDto)
    {
        await scheduleService.CreateAsync(scheduleDto);
        return Ok();
    }

    [HttpGet(nameof(Schedules))]
    public async Task<IActionResult> Schedules()
    {
        var model = await scheduleService.GetAllAsync();
        return Ok(model);
    }
}