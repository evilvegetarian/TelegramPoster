using FluentMigrator;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Migrations;

[Migration(4, $"Create Table {nameof(TelegramBot)}")]
public class CreateTelegramBot : Migration
{
    public override void Up()
    {
        Create.Table(nameof(TelegramBot))
            .WithColumn(nameof(TelegramBot.Id)).AsGuid().PrimaryKey().NotNullable()
            .WithColumn(nameof(TelegramBot.UserId)).AsGuid().NotNullable()
            .WithColumn(nameof(TelegramBot.ApiTelegram)).AsString(100).NotNullable()
            .WithColumn(nameof(TelegramBot.ChatIdWithBotUser)).AsString(100).Nullable()
            .WithColumn(nameof(TelegramBot.BotStatus)).AsInt16().NotNullable();

        Create.ForeignKey()
            .FromTable(nameof(TelegramBot)).ForeignColumn(nameof(TelegramBot.UserId))
            .ToTable(nameof(User)).PrimaryColumn(nameof(User.Id));
    }

    public override void Down()
    {
        Delete.Table(nameof(TelegramBot));
    }
}