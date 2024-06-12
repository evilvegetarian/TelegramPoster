using FluentMigrator;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Migrations;

[Migration(3, $"Create Table {nameof(FilesTelegram)}  ")]
public class CreateFilesTelegramTable : Migration
{
    public override void Up()
    {
        Create.Table(nameof(FilesTelegram))
            .WithColumn(nameof(FilesTelegram.Id)).AsGuid().NotNullable().PrimaryKey()
            .WithColumn(nameof(FilesTelegram.MessageTelegramId)).AsGuid().NotNullable()
            .WithColumn(nameof(FilesTelegram.TgFileId)).AsString().NotNullable()
            .WithColumn(nameof(FilesTelegram.Caption)).AsString(1024).Nullable()
            .WithColumn(nameof(FilesTelegram.Type)).AsInt16().NotNullable();

        Create.ForeignKey()
            .FromTable(nameof(FilesTelegram)).ForeignColumn(nameof(FilesTelegram.MessageTelegramId))
            .ToTable(nameof(MessageTelegram)).PrimaryColumn(nameof(MessageTelegram.Id));
    }

    public override void Down()
    {
        Delete.Table(nameof(FilesTelegram));
    }
}