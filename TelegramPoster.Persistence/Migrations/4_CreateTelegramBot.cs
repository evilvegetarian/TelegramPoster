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
            .WithColumn(nameof(TelegramBot.NameBot)).AsString(100).NotNullable()
            .WithColumn(nameof(TelegramBot.ChatIdWithBotUser)).AsInt64().NotNullable()
            .WithColumn(nameof(TelegramBot.BotStatus)).AsInt16().NotNullable();

        Create.ForeignKey()
            .FromTable(nameof(TelegramBot)).ForeignColumn(nameof(TelegramBot.UserId))
            .ToTable(nameof(User)).PrimaryColumn(nameof(User.Id));

        Create.Index("UX_UserId_ApiTelegram")
            .OnTable(nameof(TelegramBot))
            .OnColumn(nameof(TelegramBot.UserId)).Unique()
            .OnColumn(nameof(TelegramBot.ApiTelegram)).Unique();

        Create.ForeignKey()
            .FromTable(nameof(Schedule)).ForeignColumn(nameof(Schedule.BotId))
            .ToTable(nameof(TelegramBot)).PrimaryColumn(nameof(TelegramBot.Id));
    }

    public override void Down()
    {
        Delete.Table(nameof(TelegramBot));
    }
}