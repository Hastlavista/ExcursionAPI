namespace BlueDragon.Excursion.Core.DTOs.Auth;

public class PlanResponse
{
    public bool IsPro { get; set; }
    public int TradesThisMonth { get; set; }
    public int TradeLimit { get; set; }
}