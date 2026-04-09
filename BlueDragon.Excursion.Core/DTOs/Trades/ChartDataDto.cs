namespace BlueDragon.Excursion.Core.DTOs.Trades;

public class ChartDataDto
{
    public OhlcDataDto OhlcDataBefore { get; set; }
    public OhlcDataDto OhlcDataAfter { get; set; }
    public string ScreenshotUrlBefore { get; set; }
    public string ScreenshotUrlAfter { get; set; }
}