using TelegramPoster.Application.Interfaces;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Application.Models.Schedule;
using TelegramPoster.Auth.Interface;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Services.ScheduleServices;

public class ScheduleService(
    IScheduleRepository scheduleRepository,
    IGuidManager guidManager,
    ICurrentUserProvider currentUserProvider)
    : IScheduleService
{
    public async Task CreateAsync(ScheduleDto scheduleDto)
    {
        await scheduleRepository.AddAsync(new Schedule
        {
            Id = guidManager.NewGuid(),
            Name = scheduleDto.Name,
            ChannelId = scheduleDto.ChannelId,
            UserId = currentUserProvider.Current().UserId,
            BotId = scheduleDto.BotId
        });
    }

    public async Task<List<ScheduleView>> GetAllAsync()
    {
        var currentUserId = currentUserProvider.Current().UserId;
        var scheduleList = await scheduleRepository.GetListByUserIdAsync(currentUserId);
        return scheduleList.Select(x => new ScheduleView
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
    }
}