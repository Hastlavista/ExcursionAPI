/* Beginning Transaction */
/* Rolling back transaction */
/* 20260421000000: AlterTradesRenameProfitPipsToProfitPoints migrating ======= */

/* Beginning Transaction */
/* RenameColumn trades profit_pips to profit_points */
ALTER TABLE "excursion"."trades" RENAME COLUMN "profit_pips" TO "profit_points";
INSERT INTO "excursion"."version_info" ("version","applied_on","description") VALUES (20260421000000,(now() at time zone 'UTC'),'AlterTradesRenameProfitPipsToProfitPoints');
/* Committing Transaction */
/* 20260421000000: AlterTradesRenameProfitPipsToProfitPoints migrated */
