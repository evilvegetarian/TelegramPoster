using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TelegramPoster.Api.Middlewares;
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
        var buildConfiguration = builder.Configuration.GetSection(nameof(BuildConfiguration)).Get<BuildConfiguration>();
        builder.Services.Configure<BuildConfiguration>(builder.Configuration.GetSection(nameof(BuildConfiguration)));

        builder.Services.AddSwaggerGen(options =>
        {
            options.UseDateOnlyTimeOnlyStringConverters();
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization Reader using the Bearer scheme (\"Bearer {token} \")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API",
                Version = "v1",
                Description = $"Build Number: {buildConfiguration.BuildNumber}"
            });
        });

        builder.Services.AddHealthChecks()
            .AddUrlGroup(new Uri(cors.Front), name: "Front", tags: ["Front"]);

        builder.WebHost.UseKestrel(opt =>
        {
            opt.AddServerHeader = false;
        });

        builder.Services.AddControllers(options =>
        {
            //options.Filters.Add<HttpResponseExceptionFilter>();
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
        app.UseMiddleware<ErrorHandlingMiddleware>();
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
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}