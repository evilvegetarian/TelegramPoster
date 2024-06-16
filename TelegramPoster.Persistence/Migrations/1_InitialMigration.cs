using FluentMigrator;
using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Persistence.Migrations;

[Migration(1, "Initial Migration")]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table(nameof(User))
            .WithColumn(nameof(User.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(User.UserName)).AsString().NotNullable().Unique()
            .WithColumn(nameof(User.PasswordHash)).AsString().NotNullable()
            .WithColumn(nameof(User.Email)).AsString().NotNullable().Unique()
            .WithColumn(nameof(User.TelegramUserName)).AsString().Nullable().Unique()
            .WithColumn(nameof(User.PhoneNumber)).AsString().Nullable().Unique();

        Create.Table(nameof(Schedule))
            .WithColumn(nameof(Schedule.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Schedule.UserId)).AsGuid().NotNullable()
            .WithColumn(nameof(Schedule.Name)).AsString().NotNullable()
            .WithColumn(nameof(Schedule.ChannelId)).AsInt64().NotNullable();

        Create.Table(nameof(Day))
            .WithColumn(nameof(Day.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Day.ScheduleId)).AsGuid().NotNullable()
            .WithColumn(nameof(Day.DayOfWeek)).AsInt16().Nullable()
            .WithColumn(nameof(Day.DateDay)).AsDate().Nullable();

        Create.Table(nameof(TimePosting))
            .WithColumn(nameof(TimePosting.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(TimePosting.DayId)).AsGuid().NotNullable()
            .WithColumn(nameof(TimePosting.Time)).AsTime().NotNullable();

        Create.Index("UX_ScheduleId_DayOfWeek")
            .OnTable(nameof(Day))
            .OnColumn(nameof(Day.ScheduleId)).Unique()
            .OnColumn(nameof(Day.DayOfWeek)).Unique();

        Create.Index("UX_DayId_Time")
            .OnTable(nameof(TimePosting))
            .OnColumn(nameof(TimePosting.DayId)).Unique()
            .OnColumn(nameof(TimePosting.Time)).Unique();

        Create.ForeignKey()
            .FromTable(nameof(Day)).ForeignColumn(nameof(Day.ScheduleId))
            .ToTable(nameof(Schedule)).PrimaryColumn(nameof(Schedule.Id));

        Create.ForeignKey()
            .FromTable(nameof(TimePosting)).ForeignColumn(nameof(TimePosting.DayId))
            .ToTable(nameof(Day)).PrimaryColumn(nameof(Day.Id));

        Create.ForeignKey()
            .FromTable(nameof(Schedule)).ForeignColumn(nameof(Schedule.UserId))
            .ToTable(nameof(User)).PrimaryColumn(nameof(User.Id));
    }

    public override void Down()
    {
        Delete.Table(nameof(TimePosting));
        Delete.Table(nameof(Day));
        Delete.Table(nameof(Schedule));
        Delete.Table(nameof(User));
    }
}