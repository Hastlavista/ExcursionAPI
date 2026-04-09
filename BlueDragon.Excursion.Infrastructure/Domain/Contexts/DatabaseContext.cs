using System;
using System.Text.Json;
using BlueDragon.Excursion.Core.Enums;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueDragon.Excursion.Infrastructure.Domain.Contexts;

internal static class SerializerOptions
{
    public static readonly JsonSerializerOptions OptionsCaseInsensitive;
    public static readonly JsonSerializerOptions OptionsCamelCase;

    static SerializerOptions()
    {
        OptionsCaseInsensitive = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        OptionsCamelCase = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
    }
}

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Trade> Trades { get; set; }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("excursion");

        modelBuilder.Entity<User>().HasKey(u => new { u.Id });
        modelBuilder.Entity<Trade>().HasKey(t => new { t.Id });

        modelBuilder.Entity<Trade>()
            .Property(t => t.ChartData)
            .HasConversion(
                v => Serialize(v),
                v => Deserialize<ChartData>(v));

        modelBuilder.Entity<Trade>()
            .Property(t => t.Direction)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TradeDirection>(v));

        modelBuilder.Entity<Trade>()
            .Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TradeStatus>(v));

        base.OnModelCreating(modelBuilder);
    }

    public static DatabaseContext GenerateContext(string connectionString)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        DbContextOptionsBuilder<DatabaseContext> builder = new DbContextOptionsBuilder<DatabaseContext>();
        builder.UseNpgsql(connectionString);
        return new DatabaseContext(builder.Options);
    }

    public static string Serialize(object obj)
    {
        if (obj == null)
            return null;

        return JsonSerializer.Serialize(obj, SerializerOptions.OptionsCamelCase);
    }

    public static T Deserialize<T>(string str)
    {
        if (str == null)
            return default;

        return JsonSerializer.Deserialize<T>(str, SerializerOptions.OptionsCaseInsensitive);
    }
}
