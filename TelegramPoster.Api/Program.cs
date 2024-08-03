using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Filters;
using Swashbuckle.AspNetCore.Filters;
using TelegramPoster.Api;
using TelegramPoster.Api.Middlewares;
using TelegramPoster.Application;
using TelegramPoster.Auth;
using TelegramPoster.Persistence;
using Utility;

var builder = WebApplication.CreateBuilder(args);

var cors = builder.Configuration.GetSection(nameof(Cors)).Get<Cors>();
var buildConfiguration = builder.Configuration.GetSection(nameof(BuildConfiguration)).Get<BuildConfiguration>();
var url = builder.Configuration.GetValue<string>("Log:LogsUrl");
builder.Services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.WithProperty("Application", "TelegramPoster.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.OpenSearch(
            url,
            "telegramPoster-logs-{0:yyyy.MM.dd}"))
    .WriteTo.Logger(lc => lc.WriteTo.Console())
    .CreateLogger()));

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

builder.Services.AddApiAuthentication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddApplication()
    .AddUtility();

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
