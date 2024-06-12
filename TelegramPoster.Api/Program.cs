using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TelegramPoster.Api.Controllers;
using TelegramPoster.Application;
using TelegramPoster.Auth;
using TelegramPoster.Persistence;
using Utility;

namespace TelegramPoster.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var db = builder.Configuration.GetSection(nameof(DataBase)).Get<DataBase>();


        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks()
            .AddNpgSql(connectionString: db!.ConnectionString, name: "npsql", tags: ["npsql"])
            .AddUrlGroup(new Uri("http://example.com"), name: "Example URL", tags: ["Example"]);

        builder.WebHost.UseKestrel(opt =>
        {
            opt.AddServerHeader = false;
        });

        //ext
        builder.Services.AddApiAuthentication(builder.Configuration);

        builder.Services.AddPersistence(db!.ConnectionString);
        builder.Services.AddApplication();
        builder.Services.AddUtility();

        var app = builder.Build();
        //app.UseExceptionHandler("/api/ApiError");

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Referrer-Policy", "origin-when-cross-origin");
            await next.Invoke();
        });
        app.UseHttpsRedirection();
        app.Services.AddPersistenceServiceProvider();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health-npsql", new HealthCheckOptions
        {
            Predicate = (item) => item.Tags.Contains("npsql")
        });
        app.MapHealthChecks("/health-Example", new HealthCheckOptions
        {
            Predicate = (item) => item.Tags.Contains("Example")
        });
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Strict,
            HttpOnly = HttpOnlyPolicy.Always,
            Secure = CookieSecurePolicy.Always
        });

        app.UseCors(x =>
        {
            x.WithOrigins("http://localhost:5173");
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
