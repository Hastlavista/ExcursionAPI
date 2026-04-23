using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.Shared;
using BlueDragon.Excursion.Infrastructure.Domain.Contexts;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Implementations;

public class JournalHandler : IJournalHandler
{
    private readonly DatabaseSettings _databaseSettings;

    public JournalHandler(DatabaseSettings databaseSettings)
    {
        _databaseSettings = databaseSettings;
    }

    public async Task<List<Trade>> GetTrades(Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Trades.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<Trade> GetTrade(Guid id, Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Trades.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task CreateTrade(Trade trade)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);

        if (trade.ExternalId != null)
        {
            bool exists = await context.Trades.AnyAsync(t => t.ExternalId == trade.ExternalId);
            if (exists)
                throw new ArgumentException($"Trade with external id {trade.ExternalId} already exists");
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

    public async Task UpdateTrade(Trade update, Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        Trade existing = await context.Trades.SingleOrDefaultAsync(t => t.Id == update.Id && t.UserId == userId);
        if (existing == null)
            throw new ArgumentException($"Trade with id {update.Id} does not exist");

        existing.ExternalId = update.ExternalId ?? existing.ExternalId;
        existing.Symbol = update.Symbol ?? existing.Symbol;
        existing.Direction = update.Direction ?? existing.Direction;
        existing.EntryPrice = update.EntryPrice ?? existing.EntryPrice;
        existing.ExitPrice = update.ExitPrice ?? existing.ExitPrice;
        existing.StopLoss = update.StopLoss ?? existing.StopLoss;
        existing.TakeProfit = update.TakeProfit ?? existing.TakeProfit;
        existing.LotSize = update.LotSize ?? existing.LotSize;
        existing.Profit = update.Profit ?? existing.Profit;
        existing.Mae = update.Mae ?? existing.Mae;
        existing.Mfe = update.Mfe ?? existing.Mfe;
        existing.Efficiency = update.Efficiency ?? existing.Efficiency;
        existing.EntryTime = update.EntryTime ?? existing.EntryTime;
        existing.ExitTime = update.ExitTime ?? existing.ExitTime;
        existing.DurationMinutes = update.DurationMinutes ?? existing.DurationMinutes;
        existing.Status = update.Status ?? existing.Status;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        context.Trades.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task DeleteTrade(Guid id, Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        Trade existing = await context.Trades.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (existing == null)
            throw new ArgumentException($"Trade with id {id} does not exist");

        context.Trades.Remove(existing);
        await context.SaveChangesAsync();
    }

    public async Task UpdateScreenshot(Guid tradeId, string screenshotBefore, string screenshotAfter, Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        Trade existing = await context.Trades.SingleOrDefaultAsync(t => t.Id == tradeId && t.UserId == userId);
        if (existing == null)
            throw new ArgumentException($"Trade with id {tradeId} does not exist");

        existing.ChartData ??= new ChartData();
        existing.ChartData.ScreenshotUrlBefore = screenshotBefore ?? existing.ChartData.ScreenshotUrlBefore;
        existing.ChartData.ScreenshotUrlAfter = screenshotAfter ?? existing.ChartData.ScreenshotUrlAfter;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        context.Trades.Update(existing);
        await context.SaveChangesAsync();
    }
}
