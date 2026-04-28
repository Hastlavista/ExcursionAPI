using BlueDragon.Excursion.DatabaseMigration.Extensions;
using BlueDragon.Excursion.DatabaseMigration.Models;
using FluentMigrator;

namespace BlueDragon.Excursion.DatabaseMigration.Migrations._2026;

[DeveloperMigration(2026, 04, 13, Developer.SilvioHabazin, 0)]
public class AlterUsersAddFreeTierLimitColumns : ExcursionMigration
{
    public override void Up()
    {
        Alter.Table(Tables.Users)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("trades_this_month").AsInt32().NotNullable().WithDefaultValue(0);

        Alter.Table(Tables.Users)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("trades_reset_date").AsDateTimeOffset().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
    }
}

[DeveloperMigration(2026, 04, 13, Developer.SilvioHabazin, 1)]
public class AlterUsersCreatedAtToDateTimeOffset : ExcursionMigration
{
    public override void Up()
    {
        Alter.Table(Tables.Users)
            .InSchema(Tables.Schemas.Excursion)
            .AlterColumn("created_at").AsDateTimeOffset().NotNullable();
    }
}

[DeveloperMigration(2026, 04, 13, Developer.SilvioHabazin, 2)]
public class AlterTradesEntryExitTimeToDateTimeOffset : ExcursionMigration
{
    public override void Up()
    {
        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AlterColumn("entry_time").AsDateTimeOffset().NotNullable();

        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AlterColumn("exit_time").AsDateTimeOffset().Nullable();
    }
}

[DeveloperMigration(2026, 04, 21, Developer.SilvioHabazin, 0)]
public class AlterTradesRenameProfitPipsToProfitPoints : ExcursionMigration
{
    public override void Up()
    {
        Rename.Column("profit_pips").OnTable(Tables.Trades).InSchema(Tables.Schemas.Excursion).To("profit_points");
    }
}

[DeveloperMigration(2026, 04, 23, Developer.SilvioHabazin, 0)]
public class AlterTradesAddUniqueExternalId : ExcursionMigration
{
    public override void Up()
    {
        Execute.Sql("CREATE UNIQUE INDEX uq_trades_external_id ON excursion.trades (external_id) WHERE external_id IS NOT NULL;");
    }
}

[DeveloperMigration(2026, 04, 23, Developer.SilvioHabazin, 1)]
public class AlterTradesProfitPointsPrecision : ExcursionMigration
{
    public override void Up()
    {
        Alter.Table(Tables.Trades)
            .InSchema(Tables.Schemas.Excursion)
            .AlterColumn("profit_points").AsDecimal().Nullable();
    }
}

[DeveloperMigration(2026, 04, 28, Developer.SilvioHabazin, 0)]
public class AlterUsersAddStripeColumns : ExcursionMigration
{
    public override void Up()
    {
        Alter.Table(Tables.Users)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("stripe_customer_id").AsString().Nullable();

        Alter.Table(Tables.Users)
            .InSchema(Tables.Schemas.Excursion)
            .AddColumn("stripe_subscription_id").AsString().Nullable();
    }
}