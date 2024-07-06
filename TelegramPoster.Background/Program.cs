using Hangfire;
using TelegramPoster.Auth;
using TelegramPoster.Persistence;

namespace TelegramPoster.Background;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage();
        });

        builder.Services.AddHangfireServer();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<MessageService>();

        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddApiAuthentication(builder.Configuration);

        var app = builder.Build();

        app.UseHangfireDashboard();

        var serviceProvider = app.Services;

        RecurringJob.AddOrUpdate(
        "process-messages-job",
            () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<MessageService>().ProcessMessages(),
        "* * * * *");

        app.MapGet("/", context =>
        {
            context.Response.Redirect("hangfire");
            return Task.CompletedTask;
        });

        app.Run();
    }
}