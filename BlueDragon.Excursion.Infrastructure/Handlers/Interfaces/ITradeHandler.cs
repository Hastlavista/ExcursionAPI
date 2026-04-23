using System.Threading.Tasks;
using BlueDragon.Excursion.Infrastructure.Domain.Models;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;

public interface ITradeHandler
{
    Task AddTrade(Trade trade);
    Task UpdateTrade(Trade update);
    Task CloseTrade(Trade update);
}
