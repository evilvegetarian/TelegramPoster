using Hangfire;
using Hangfire.Dashboard;
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

        var tempAuth = builder.Configuration.GetSection(nameof(TempAuth)).Get<TempAuth>();

        var app = builder.Build();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new BasicAuthAuthorizationFilter(tempAuth.Password, app.Environment.IsDevelopment()) }
        });

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
public class TempAuth
{
    public string Password { get; set; }
}

public class BasicAuthAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string value;
    private readonly bool isdev;

    public BasicAuthAuthorizationFilter(string value, bool isdev)
    {
        this.value = value;
        this.isdev = isdev;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var cookieValue = httpContext.Request.Cookies["Authorization"];
        if (value == cookieValue || isdev)
        {
            return true;

        }
        return false;
    }
}