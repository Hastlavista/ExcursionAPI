using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlueDragon.Excursion.Infrastructure.Domain.Models;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;

public interface IJournalHandler
{
    Task<List<Trade>> GetTrades(Guid userId);
    Task<Trade> GetTrade(Guid id, Guid userId);
    Task CreateTrade(Trade trade);
    Task UpdateTrade(Trade update, Guid userId);
    Task DeleteTrade(Guid id, Guid userId);
    Task UpdateScreenshot(Guid tradeId, string screenshotBefore, string screenshotAfter, Guid userId);
}
