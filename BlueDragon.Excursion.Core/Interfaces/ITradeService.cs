using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Trades;

namespace BlueDragon.Excursion.Core.Interfaces;

public interface ITradeService
{
    Task OpenTrade(TradeDto tradeDto, Guid userId);
    Task CloseTrade(TradeDto update);
    Task<List<TradeBaseDto>> GetTrades(Guid userId);
    Task<TradeDto> GetTrade(Guid id, Guid userId);
    Task UpdateTrade(TradeDto update);
}