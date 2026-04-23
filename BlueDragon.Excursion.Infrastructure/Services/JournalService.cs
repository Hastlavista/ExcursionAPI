using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Requests;
using BlueDragon.Excursion.Core.DTOs.Trades;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using BlueDragon.Excursion.Infrastructure.Mappers;

namespace BlueDragon.Excursion.Infrastructure.Services;

public class JournalService : IJournalService
{
    private readonly IJournalHandler _journalHandler;

    public JournalService(IJournalHandler journalHandler)
    {
        _journalHandler = journalHandler;
    }

    public async Task<List<TradeBaseDto>> GetTrades(Guid userId)
    {
        List<Trade> trades = await _journalHandler.GetTrades(userId);
        return trades.Select(t => t.ToBaseDto()).ToList();
    }

    public async Task<TradeDto> GetTrade(Guid id, Guid userId)
    {
        Trade trade = await _journalHandler.GetTrade(id, userId);
        return trade.ToDto();
    }

    public async Task CreateTrade(TradeDto tradeDto, Guid userId)
    {
        Trade trade = tradeDto.ToDomain();
        trade.UserId = userId;
        await _journalHandler.CreateTrade(trade);
    }

    public async Task UpdateTrade(TradeDto update, Guid userId)
    {
        Trade trade = update.ToDomain();
        await _journalHandler.UpdateTrade(trade, userId);
    }

    public async Task DeleteTrade(Guid id, Guid userId)
    {
        await _journalHandler.DeleteTrade(id, userId);
    }

    public async Task UpdateScreenshots(UpdateScreenshotsRequest request, Guid userId)
    {
        await _journalHandler.UpdateScreenshot(request.Id.GetValueOrDefault(), request.ScreenshotBefore, request.ScreenshotAfter, userId);
    }
}
