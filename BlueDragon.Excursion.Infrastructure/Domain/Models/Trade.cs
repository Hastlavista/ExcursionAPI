using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlueDragon.Excursion.Core.Enums;

namespace BlueDragon.Excursion.Infrastructure.Domain.Models;

[Table("trades")]
public class Trade
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid? Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("external_id")]
    public long? ExternalId { get; set; }

    [Column("symbol")]
    public string Symbol { get; set; }

    [Column("direction")]
    public TradeDirection? Direction { get; set; }

    [Column("entry_price")]
    public decimal? EntryPrice { get; set; }

    [Column("exit_price")]
    public decimal? ExitPrice { get; set; }

    [Column("stop_loss")]
    public decimal? StopLoss { get; set; }

    [Column("take_profit")]
    public decimal? TakeProfit { get; set; }

    [Column("lot_size")]
    public decimal? LotSize { get; set; }

    [Column("profit")]
    public decimal? Profit { get; set; }

    [Column("profit_pips")]
    public decimal? ProfitPips { get; set; }

    [Column("mae")]
    public decimal? Mae { get; set; }

    [Column("mfe")]
    public decimal? Mfe { get; set; }

    [Column("efficiency")]
    public decimal? Efficiency { get; set; }

    [Column("entry_time")]
    public DateTimeOffset? EntryTime { get; set; }

    [Column("exit_time")]
    public DateTimeOffset? ExitTime { get; set; }

    [Column("duration_minutes")]
    public int? DurationMinutes { get; set; }

    [Column("chart_data", TypeName = "jsonb")]
    public ChartData ChartData { get; set; }

    [Column("status")]
    public TradeStatus? Status { get; set; }
    
    [Column("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    [Column("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }
}

public class ChartData
{
    public OhlcData OhlcDataBefore { get; set; }
    public OhlcData OhlcDataAfter { get; set; }
    public string ScreenshotUrlBefore { get; set; }
    public string ScreenshotUrlAfter { get; set; }
}

public class OhlcData
{
    public string Timeframe { get; set; }
    public DateTime? EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public IList<Candle> Candles { get; set; }
}

public class Candle
{
    public long Time { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
}
