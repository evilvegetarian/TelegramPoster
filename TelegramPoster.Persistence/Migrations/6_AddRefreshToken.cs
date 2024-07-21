using FluentMigrator;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Migrations;

[Migration(6, $"Add Refresh Token in User")]
public class AddRefreshToken :Migration
{
    public override void Up()
    {
        Alter.Table(nameof(User))
            .AddColumn(nameof(User.RefreshToken)).AsString().Nullable()
            .AddColumn(nameof(User.RefreshTokenExpiryTime)).AsString().Nullable();
    }

    public override void Down()
    {
        Delete
            .Column(nameof(User.RefreshToken))
            .Column(nameof(User.RefreshTokenExpiryTime))
            .FromTable(nameof(User));
    }
}