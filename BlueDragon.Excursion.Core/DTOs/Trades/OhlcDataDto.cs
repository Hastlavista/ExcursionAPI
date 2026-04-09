using System;
using System.Collections.Generic;

namespace BlueDragon.Excursion.Core.DTOs.Trades;

public class OhlcDataDto
{
    public string Timeframe { get; set; }
    public DateTime? EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public IList<CandleDto> Candles { get; set; }
}
