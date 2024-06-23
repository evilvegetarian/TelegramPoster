using TelegramPoster.Persistence;

namespace TelegramPoster.Background;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddHostedService<MessageService>();

        var host = builder.Build();
        host.Run();
    }
}