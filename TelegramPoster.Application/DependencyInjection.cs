using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace TelegramPoster.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes
                .Where(type => type.IsClass && (type.Name.EndsWith("Service")|| type.Name.EndsWith("Validator")))
            )
            .UsingRegistrationStrategy(RegistrationStrategy.Throw)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }
}