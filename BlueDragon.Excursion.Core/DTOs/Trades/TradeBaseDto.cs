using System;
using BlueDragon.Excursion.Core.Enums;

namespace BlueDragon.Excursion.Core.DTOs.Trades;

public class TradeBaseDto
{
    public Guid? Id { get; set; }
    public long? ExternalId { get; set; }
    public string Symbol { get; set; }
    public TradeDirection? Direction { get; set; }
    public decimal? EntryPrice { get; set; }
    public decimal? ExitPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public decimal? LotSize { get; set; }
    public decimal? Profit { get; set; }
    public decimal? ProfitPips { get; set; }
    public decimal? Mae { get; set; }
    public decimal? Mfe { get; set; }
    public decimal? Efficiency { get; set; }
    public DateTime? EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public int? DurationMinutes { get; set; }
    public TradeStatus? Status { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
}