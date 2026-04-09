using System;
using System.Collections.Generic;
using System.Linq;
using BlueDragon.Excursion.Core.Enums;
using BlueDragon.Excursion.Infrastructure.Domain.Models;

namespace BlueDragon.Excursion.Infrastructure.Utils;

public static class TradeUtils
{
    public static decimal? CalculateProfitPips(string symbol, TradeDirection? direction, decimal? entryPrice, decimal? exitPrice)
    {
        if (direction == null || entryPrice == null || exitPrice == null) 
            return null;
        
        decimal? priceDiff = direction == TradeDirection.Buy
            ? exitPrice - entryPrice
            : entryPrice - exitPrice;

        return priceDiff * GetPipMultiplier(symbol);
    }

    public static decimal? CalculateMae(TradeDirection? direction, decimal? entryPrice, IList<Candle> candles)
    {
        if (direction == null ||  entryPrice == null || candles == null)
            return null;
        
        decimal lowestLow   = candles.Min(c => c.Low);
        decimal highestHigh = candles.Max(c => c.High);

        return direction == TradeDirection.Buy
            ? entryPrice - lowestLow
            : highestHigh - entryPrice;
    }

    public static decimal? CalculateMfe(TradeDirection? direction, decimal? entryPrice, IList<Candle> candles)
    {
        if (direction == null ||  entryPrice == null || candles == null)
            return null;
        
        decimal lowestLow   = candles.Min(c => c.Low);
        decimal highestHigh = candles.Max(c => c.High);

        return direction == TradeDirection.Buy
            ? highestHigh - entryPrice
            : entryPrice - lowestLow;
    }

    public static decimal? CalculateEfficiency(decimal? mfe, decimal? entryPrice, decimal? exitPrice, TradeDirection? direction)
    {
        if (!mfe.HasValue || mfe <= 0)
            return null;

        if (!entryPrice.HasValue || !exitPrice.HasValue || !direction.HasValue)
            return null;

        decimal profitInPrice = direction == TradeDirection.Buy
            ? exitPrice.Value - entryPrice.Value
            : entryPrice.Value - exitPrice.Value;

        decimal efficiency = (profitInPrice / mfe.Value) * 100;
        return Math.Round(efficiency, 2);
    }

    private static int GetPipMultiplier(string symbol)
    {
        if (string.IsNullOrEmpty(symbol)) return 10000;
        return symbol.Contains("JPY") ? 100 : 10000;
    }
}