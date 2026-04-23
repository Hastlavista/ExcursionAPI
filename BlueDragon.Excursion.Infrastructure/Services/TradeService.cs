using System;
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

    public async Task UpdateTrade(TradeDto update)
    {
        Trade trade = update.ToDomain();
        await _tradeHandler.UpdateTrade(trade);
    }
}
