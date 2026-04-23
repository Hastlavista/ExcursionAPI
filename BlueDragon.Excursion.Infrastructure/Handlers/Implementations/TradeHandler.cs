using System;
using System.Linq;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.Enums;
using BlueDragon.Excursion.Core.Shared;
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
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);

        if (trade.ExternalId != null)
        {
            bool exists = await context.Trades.AnyAsync(t => t.ExternalId == trade.ExternalId);
            if (exists)
                return;
        }

        User user = await context.Users.SingleOrDefaultAsync(u => u.Id == trade.UserId);

        DateTimeOffset today = DateTimeOffset.UtcNow;
        if (user.TradesResetDate.Year != today.Year || user.TradesResetDate.Month != today.Month)
        {
            user.TradesThisMonth = 0;
            user.TradesResetDate = today;
        }

        if (user.IsPro != true && user.TradesThisMonth >= PlanConstants.MonthlyTradeLimit)
            throw new TradeLimitExceededException();

        user.TradesThisMonth++;
        context.Users.Update(user);

        DateTimeOffset now = DateTimeOffset.UtcNow;
        trade.CreatedAt = now;
        trade.UpdatedAt = now;
        context.Trades.Add(trade);

        await context.SaveChangesAsync();
    }

    public async Task UpdateTrade(Trade update)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        Trade existing = context.Trades.FirstOrDefault(t => t.ExternalId == update.ExternalId);
        if (existing == null)
            throw new ArgumentException($"Trade with external id {update.ExternalId} does not exist");

        existing.StopLoss = update.StopLoss;
        existing.TakeProfit = update.TakeProfit;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        context.Trades.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task CloseTrade(Trade update)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        Trade existing = context.Trades.FirstOrDefault(t => update.Id != null ? t.Id == update.Id : t.ExternalId == update.ExternalId);
        if (existing == null)
            throw new ArgumentException($"Trade with id {update.Id} / external id {update.ExternalId} does not exist");

        existing.ExitPrice = update.ExitPrice;
        existing.Profit = update.Profit;
        existing.ExitTime = update.ExitTime;
        existing.ProfitPoints = TradeUtils.CalculateProfitPoints(existing.Direction, existing.EntryPrice, existing.ExitPrice);
        existing.Mae = TradeUtils.CalculateMae(existing.Direction, existing.EntryPrice, update.ChartData?.OhlcDataAfter?.Candles);
        existing.Mfe = TradeUtils.CalculateMfe(existing.Direction, existing.EntryPrice, update.ChartData?.OhlcDataAfter?.Candles);
        existing.ChartData ??= new ChartData();
        existing.ChartData.OhlcDataAfter = update.ChartData?.OhlcDataAfter ?? existing.ChartData.OhlcDataAfter;
        existing.ChartData.ScreenshotUrlAfter = update.ChartData?.ScreenshotUrlAfter ?? existing.ChartData.ScreenshotUrlAfter;
        existing.ChartData.OhlcDataBefore = update.ChartData?.OhlcDataBefore ?? existing.ChartData.OhlcDataBefore;
        existing.ChartData.ScreenshotUrlBefore = update.ChartData?.ScreenshotUrlBefore ?? existing.ChartData.ScreenshotUrlBefore;
        existing.Efficiency = TradeUtils.CalculateEfficiency(existing.Mfe, existing.EntryPrice, existing.ExitPrice, existing.Direction);
        existing.Status = TradeStatus.Closed;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        if (existing.EntryTime.HasValue && existing.ExitTime.HasValue)
            existing.DurationMinutes = (int)(existing.ExitTime.Value - existing.EntryTime.Value).TotalMinutes;

        context.Trades.Update(existing);
        await context.SaveChangesAsync();
    }
}
