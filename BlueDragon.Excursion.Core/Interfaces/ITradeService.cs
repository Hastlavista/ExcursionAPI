using System;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Trades;

namespace BlueDragon.Excursion.Core.Interfaces;

public interface ITradeService
{
    Task OpenTrade(TradeDto tradeDto, Guid userId);
    Task CloseTrade(TradeDto update);
    Task UpdateTrade(TradeDto update);
}
