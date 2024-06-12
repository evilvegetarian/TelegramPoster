using FluentMigrator;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Migrations;

[Migration(5, $"Create Table {nameof(CreateBotChannelLink)}")]
public class CreateBotChannelLink : Migration
{
    public override void Up()
    {
        Create.Table(nameof(BotChannelLink))
            .WithColumn(nameof(BotChannelLink.BotId)).AsGuid().NotNullable()
            .WithColumn(nameof(BotChannelLink.ChannelId)).AsInt64().NotNullable();

        Create.PrimaryKey("PK_BotChannelLink")
            .OnTable(nameof(BotChannelLink)).Columns(nameof(BotChannelLink.BotId), nameof(BotChannelLink.ChannelId));
    }
    public override void Down()
    {
        Delete.Table(nameof(BotChannelLink));
    }
}
