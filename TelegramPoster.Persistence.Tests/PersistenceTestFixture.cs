using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace TelegramPoster.Persistence.Tests;

public class PersistenceTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder().Build();
    private IServiceProvider serviceProvider;
    public IServiceProvider ServiceProvider => serviceProvider;

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();
        var connectionString = dbContainer.GetConnectionString();

        var services = new ServiceCollection();
        services.AddPersistence(new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string>("DataBase:ConnectionString", connectionString)
            ])
            .Build());

        serviceProvider = services.BuildServiceProvider();
        serviceProvider.AddPersistenceServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await dbContainer.StopAsync();
    }
}