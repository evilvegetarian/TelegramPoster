using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;
using System.Data;
using TelegramPoster.Persistence.Context;

namespace TelegramPoster.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataBase = configuration.GetSection(nameof(DataBase)).Get<DataBase>();
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(run => run
                .AddPostgres()
                .WithGlobalConnectionString(dataBase!.ConnectionString)
                .ScanIn(typeof(DependencyInjection).Assembly).For.Migrations())
            .AddLogging(lb =>
            {
                lb.AddConsole();
            });

        SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());

        services.AddSingleton<ISqlConnectionFactory>(_ => new PostgreSqlConnectionFactory(dataBase!.ConnectionString));

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
        //serviceProvider.RevertDatabaseToVersion(0);
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

    public class SqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly time)
        {
            parameter.Value = time.ToTimeSpan();
        }

        public override TimeOnly Parse(object value)
        {
            return TimeOnly.FromTimeSpan((TimeSpan)value);
        }
    }

    public class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly date)
        {
            parameter.Value = date.ToDateTime(new TimeOnly(0, 0));
            parameter.DbType = DbType.Date;
        }

        public override DateOnly Parse(object value)
        {
            return DateOnly.FromDateTime((DateTime)value);
        }
    }
}

public class DataBase
{
    public required string ConnectionString { get; init; }
}
