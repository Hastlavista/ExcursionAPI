using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.Enums;
using BlueDragon.Excursion.Infrastructure.Domain.Contexts;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using BlueDragon.Excursion.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Implementations;

public class TradeHandler : ITradeHandler
{
    private readonly DatabaseSettings _databaseSettings;

    public TradeHandler(DatabaseSettings databaseSettings)
    {
        _databaseSettings = databaseSettings;
    }

    public async Task AddTrade(Trade trade)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        trade.CreatedAt = now;
        trade.UpdatedAt = now;
        
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        context.Trades.Add(trade);
        await context.SaveChangesAsync();
    }

    public async Task<Trade> GetTrade(Guid id, Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Trades.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<List<Trade>> GetTrades(Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Trades.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task UpdateTrade(Trade update)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        Trade existing = context.Trades.SingleOrDefault(t => update.ExternalId != null ? t.ExternalId == update.ExternalId : t.Id == update.Id);
        if (existing == null)
            throw new ArgumentException($"Trade with id {update.Id} / external id {update.ExternalId} does not exist");

        existing.TakeProfit = update.TakeProfit;
        existing.StopLoss = update.StopLoss;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        context.Trades.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task CloseTrade(Trade update)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        Trade existing = context.Trades.SingleOrDefault(t => update.ExternalId != null ? t.ExternalId == update.ExternalId : t.Id == update.Id);
        if (existing == null)
            throw new ArgumentException($"Trade with id {update.Id} / external id {update.ExternalId} does not exist");

        existing.ExitPrice = update.ExitPrice;
        existing.Profit = update.Profit;
        existing.ProfitPips  = TradeUtils.CalculateProfitPips(existing.Symbol, existing.Direction, existing.EntryPrice, existing.ExitPrice);
        existing.Mae = TradeUtils.CalculateMae(existing.Direction, existing.EntryPrice, update.ChartData?.OhlcDataAfter?.Candles);
        existing.Mfe = TradeUtils.CalculateMfe(existing.Direction, existing.EntryPrice,  update.ChartData?.OhlcDataAfter?.Candles);
        existing.ChartData ??= new ChartData();
        existing.ChartData.OhlcDataAfter = update.ChartData?.OhlcDataAfter ?? existing.ChartData.OhlcDataAfter;
        existing.ChartData.ScreenshotUrlAfter = update.ChartData?.ScreenshotUrlAfter ?? existing.ChartData.ScreenshotUrlAfter;
        existing.ChartData.OhlcDataBefore = update.ChartData?.OhlcDataBefore ?? existing.ChartData.OhlcDataBefore;
        existing.ChartData.ScreenshotUrlBefore = update.ChartData?.ScreenshotUrlBefore ?? existing.ChartData.ScreenshotUrlBefore;
        existing.Efficiency = TradeUtils.CalculateEfficiency(existing.Mfe, existing.EntryPrice, existing.ExitPrice, existing.Direction);
        existing.ExitTime = update.ExitTime;
        existing.Status = TradeStatus.Closed;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        if (existing.EntryTime.HasValue && existing.ExitTime.HasValue)
            existing.DurationMinutes = (int)(existing.ExitTime.Value - existing.EntryTime.Value).TotalMinutes;

        context.Trades.Update(existing);
        await context.SaveChangesAsync();
    }
}
