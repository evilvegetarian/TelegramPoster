using FluentMigrator;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Migrations;

[Migration(2, $"Create Table {nameof(MessageTelegram)}")]
public class CreateTelegramMessageTable : Migration
{
    public override void Up()
    {
        Create.Table(nameof(MessageTelegram))
            .WithColumn(nameof(MessageTelegram.Id)).AsGuid().NotNullable().PrimaryKey()
            .WithColumn(nameof(MessageTelegram.ScheduleId)).AsGuid().NotNullable()
            .WithColumn(nameof(MessageTelegram.TimePosting)).AsDateTime().NotNullable()
            .WithColumn(nameof(MessageTelegram.TextMessage)).AsString(4096).Nullable()
            .WithColumn(nameof(MessageTelegram.IsTextMessage)).AsBoolean().NotNullable()
            .WithColumn(nameof(MessageTelegram.Status)).AsInt16().NotNullable();

        Create.ForeignKey()
            .FromTable(nameof(MessageTelegram)).ForeignColumn(nameof(MessageTelegram.ScheduleId))
            .ToTable(nameof(Schedule)).PrimaryColumn(nameof(Schedule.Id));
    }

    public override void Down()
    {
        Delete.Table(nameof(MessageTelegram));
    }
}