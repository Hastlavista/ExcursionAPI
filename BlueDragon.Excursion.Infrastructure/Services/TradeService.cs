using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Trades;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using BlueDragon.Excursion.Infrastructure.Mappers;

namespace BlueDragon.Excursion.Infrastructure.Services;

public class TradeService : ITradeService
{
    private readonly ITradeHandler _tradeHandler;

    public TradeService(ITradeHandler tradeHandler)
    {
        _tradeHandler = tradeHandler;
    }

    public async Task OpenTrade(TradeDto tradeDto, Guid userId)
    {
        Trade domain = tradeDto.ToDomain();
        domain.UserId = userId;
        await _tradeHandler.AddTrade(domain);
    }

    public async Task CloseTrade(TradeDto update)
    {
        Trade tradeUpdate = update.ToDomain();
        await _tradeHandler.CloseTrade(tradeUpdate);
    }

    public async Task<List<TradeBaseDto>> GetTrades(Guid userId)
    {
        List<Trade> trades = await _tradeHandler.GetTrades(userId);
        return trades.Select(t => t.ToBaseDto()).ToList();
    }

    public async Task<TradeDto> GetTrade(Guid id, Guid userId)
    {
        Trade trade = await _tradeHandler.GetTrade(id, userId);
        return trade.ToDto();
    }

    public async Task UpdateTrade(TradeDto update)
    {
        Trade trade = update.ToDomain();
        await _tradeHandler.UpdateTrade(trade);
    }
}