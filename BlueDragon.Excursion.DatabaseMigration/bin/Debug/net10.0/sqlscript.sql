/* Beginning Transaction */
/* Rolling back transaction */
/* 20260413000000: AlterUsersAddFreeTierLimitColumns migrating =============== */

/* Beginning Transaction */
/* AlterTable users */
/* No SQL statement executed. */
/* CreateColumn users trades_this_month Int32 */
ALTER TABLE "excursion"."users" ADD "trades_this_month" integer NOT NULL DEFAULT 0;
/* AlterTable users */
/* No SQL statement executed. */
/* CreateColumn users trades_reset_date DateTimeOffset */
ALTER TABLE "excursion"."users" ADD "trades_reset_date" timestamptz NOT NULL DEFAULT now();
INSERT INTO "excursion"."version_info" ("version","applied_on","description") VALUES (20260413000000,(now() at time zone 'UTC'),'AlterUsersAddFreeTierLimitColumns');
/* Committing Transaction */
/* 20260413000000: AlterUsersAddFreeTierLimitColumns migrated */
/* 20260413000001: AlterUsersCreatedAtToDateTimeOffset migrating ============= */

/* Beginning Transaction */
/* AlterTable users */
/* No SQL statement executed. */
/* AlterColumn users created_at DateTimeOffset */
ALTER TABLE "excursion"."users" ALTER "created_at" TYPE timestamptz, ALTER "created_at" SET NOT NULL;
INSERT INTO "excursion"."version_info" ("version","applied_on","description") VALUES (20260413000001,(now() at time zone 'UTC'),'AlterUsersCreatedAtToDateTimeOffset');
/* Committing Transaction */
/* 20260413000001: AlterUsersCreatedAtToDateTimeOffset migrated */
/* 20260413000002: AlterTradesEntryExitTimeToDateTimeOffset migrating ======== */

/* Beginning Transaction */
/* AlterTable trades */
/* No SQL statement executed. */
/* AlterColumn trades entry_time DateTimeOffset */
ALTER TABLE "excursion"."trades" ALTER "entry_time" TYPE timestamptz, ALTER "entry_time" SET NOT NULL;
/* AlterTable trades */
/* No SQL statement executed. */
/* AlterColumn trades exit_time DateTimeOffset */
ALTER TABLE "excursion"."trades" ALTER "exit_time" TYPE timestamptz, ALTER "exit_time" DROP NOT NULL;
INSERT INTO "excursion"."version_info" ("version","applied_on","description") VALUES (20260413000002,(now() at time zone 'UTC'),'AlterTradesEntryExitTimeToDateTimeOffset');
/* Committing Transaction */
/* 20260413000002: AlterTradesEntryExitTimeToDateTimeOffset migrated */
