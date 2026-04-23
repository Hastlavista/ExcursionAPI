using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Requests;
using BlueDragon.Excursion.Core.DTOs.Trades;

namespace BlueDragon.Excursion.Core.Interfaces;

public interface IJournalService
{
    Task<List<TradeBaseDto>> GetTrades(Guid userId);
    Task<TradeDto> GetTrade(Guid id, Guid userId);
    Task CreateTrade(TradeDto trade, Guid userId);
    Task UpdateTrade(TradeDto update, Guid userId);
    Task DeleteTrade(Guid id, Guid userId);
    Task UpdateScreenshots(UpdateScreenshotsRequest request, Guid userId);
}
