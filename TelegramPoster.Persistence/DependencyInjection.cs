using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;
using TelegramPoster.Persistence.Context;

namespace TelegramPoster.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(run => run
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(DependencyInjection).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddConsole());

        services.AddSingleton<ISqlConnectionFactory>(_ => new PostgreSqlConnectionFactory(connectionString));

        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes
                .Where(type => type.IsClass && type.Name.EndsWith("Repository"))
            )
            .UsingRegistrationStrategy(RegistrationStrategy.Throw)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }

    public static IServiceProvider AddPersistenceServiceProvider(this IServiceProvider serviceProvider)
    {
        serviceProvider.RevertDatabaseToVersion(3);
        serviceProvider.UpdateDatabase();
        return serviceProvider;
    }

    private static void UpdateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private static void RevertDatabaseToVersion(this IServiceProvider serviceProvider, long targetVersion)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(targetVersion);
    }
}

public class DataBase
{
    public required string ConnectionString { get; init; }
}
