using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TelegramPoster.Application;
using TelegramPoster.Auth;
using TelegramPoster.Persistence;
using Utility;
using static TelegramPoster.Application.Validator.ValidationExtensions;

namespace TelegramPoster.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var cors = builder.Configuration.GetSection(nameof(Cors)).Get<Cors>();

        builder.Services.AddSwaggerGen(options =>
        {
            options.UseDateOnlyTimeOnlyStringConverters();
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks()
            .AddUrlGroup(new Uri(cors.Front), name: "Front", tags: ["Front"]);

        builder.WebHost.UseKestrel(opt =>
        {
            opt.AddServerHeader = false;
        });

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<HttpResponseExceptionFilter>();
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(cors.Front)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        //ext
        builder.Services.AddApiAuthentication(builder.Configuration);

        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddUtility();

        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Referrer-Policy", "origin-when-cross-origin");
            await next.Invoke();
        });
        app.UseHttpsRedirection();
        app.Services.AddPersistenceServiceProvider();

            app.UseSwagger();
            app.UseSwaggerUI();
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
        //app.Services.SaveSwaggerJson();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}