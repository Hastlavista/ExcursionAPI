using BlueDragon.Excursion.DatabaseMigration.Extensions;
using BlueDragon.Excursion.DatabaseMigration.Models;
using FluentMigrator;

namespace BlueDragon.Excursion.DatabaseMigration.Migrations._2026;

[DeveloperMigration(2026, 03, 23, Developer.SilvioHabazin, 0)]
public class CreateExcursionSchema : ExcursionMigration
{
    public override void Up()
    {
        Execute.Sql("CREATE SCHEMA IF NOT EXISTS excursion;");
    }
}

[DeveloperMigration(2026, 03, 23, Developer.SilvioHabazin, 1)]
public class CreateUsersTable : ExcursionMigration
{
    public override void Up()
    {
        Create.Table(Tables.Users)
            .InSchema(Tables.Schemas.Excursion)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_users")
            .WithColumn("email").AsString(255).NotNullable().Unique("uq_users_email")
            .WithColumn("password_hash").AsString(255).NotNullable()
            .WithColumn("api_key").AsString(255).NotNullable().Unique("uq_users_api_key")
            .WithColumn("is_pro").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("created_at").AsDateTime().NotNullable();
    }
}

[DeveloperMigration(2026, 03, 23, Developer.SilvioHabazin, 2)]
public class CreateTradesTable : ExcursionMigration
{
    public override void Up()
    {
        Create.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey("pk_trades")
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("symbol").AsString(20).NotNullable()
            .WithColumn("direction").AsString(4).NotNullable()
            .WithColumn("entry_price").AsDecimal(18, 5).NotNullable()
            .WithColumn("exit_price").AsDecimal(18, 5).Nullable()
            .WithColumn("stop_loss").AsDecimal(18, 5).Nullable()
            .WithColumn("take_profit").AsDecimal(18, 5).Nullable()
            .WithColumn("lot_size").AsDecimal(10, 2).NotNullable()
            .WithColumn("profit").AsDecimal(18, 2).Nullable()
            .WithColumn("profit_pips").AsDecimal(10, 1).Nullable()
            .WithColumn("mae").AsDecimal(18, 5).Nullable()
            .WithColumn("mfe").AsDecimal(18, 5).Nullable()
            .WithColumn("efficiency").AsDecimal(5, 2).Nullable()
            .WithColumn("entry_time").AsDateTime().NotNullable()
            .WithColumn("exit_time").AsDateTime().Nullable()
            .WithColumn("duration_minutes").AsInt32().Nullable()
            .WithColumn("ohlc_data").AsCustom("jsonb").Nullable()
            .WithColumn("status").AsString(10).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable();

        Create.ForeignKey("fk_trades_user_id")
            .FromTable(Tables.Trades).InSchema(Tables.Schemas.Excursion).ForeignColumn("user_id")
            .ToTable(Tables.Users).InSchema(Tables.Schemas.Excursion).PrimaryColumn("id");
    }
}

[DeveloperMigration(2026, 03, 25, Developer.SilvioHabazin, 0)]
public class AlterTradesAddUpdatedAt : ExcursionMigration
{
    public override void Up()
    {
        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AlterColumn("created_at").AsDateTimeOffset().NotNullable();

        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("updated_at").AsDateTimeOffset().NotNullable();
    }
}

[DeveloperMigration(2026, 03, 26, Developer.SilvioHabazin, 0)]
public class AlterTradesAddExternalId : ExcursionMigration
{
    public override void Up()
    {
        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("external_id").AsString(50).Nullable();
    }
}

[DeveloperMigration(2026, 03, 26, Developer.SilvioHabazin, 1)]
public class AlterTradesExternalIdToLong : ExcursionMigration
{
    public override void Up()
    {
        Delete.Column("external_id").FromTable(Tables.Trades).InSchema(Tables.Schemas.Excursion);

        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("external_id").AsInt64().Nullable();
    }
}

[DeveloperMigration(2026, 03, 26, Developer.SilvioHabazin, 2)]
public class AlterTradesReplaceOhlcDataWithChartData : ExcursionMigration
{
    public override void Up()
    {
        Delete.Column("ohlc_data").FromTable(Tables.Trades).InSchema(Tables.Schemas.Excursion);

        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("chart_data").AsCustom("jsonb").Nullable();
    }
}
