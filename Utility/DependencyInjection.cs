using Microsoft.Extensions.DependencyInjection;
using TelegramPoster.Application.Interfaces;
using Utility.GuidHelper;

namespace Utility;

public static class DependencyInjection
{
    public static IServiceCollection AddUtility(this IServiceCollection services)
    {
        services.AddScoped<IGuidManager, SequentialGuidManager>();
        return services;
    }
}