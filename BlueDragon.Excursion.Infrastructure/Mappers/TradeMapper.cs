using System.Linq;
using BlueDragon.Excursion.Core.DTOs.Trades;
using BlueDragon.Excursion.Infrastructure.Domain.Models;

namespace BlueDragon.Excursion.Infrastructure.Mappers;

public static class TradeMapper
{
    #region DTO

    public static TradeBaseDto ToBaseDto(this Trade trade)
    {
        if (trade == null) return null;

        return new TradeBaseDto
        {
            Id = trade.Id,
            ExternalId = trade.ExternalId,
            Symbol = trade.Symbol,
            Direction = trade.Direction,
            EntryPrice = trade.EntryPrice,
            ExitPrice = trade.ExitPrice,
            StopLoss = trade.StopLoss,
            TakeProfit = trade.TakeProfit,
            LotSize = trade.LotSize,
            Profit = trade.Profit,
            ProfitPips = trade.ProfitPips,
            Mae = trade.Mae,
            Mfe = trade.Mfe,
            Efficiency = trade.Efficiency,
            EntryTime = trade.EntryTime,
            ExitTime = trade.ExitTime,
            DurationMinutes = trade.DurationMinutes,
            Status = trade.Status,
            UpdatedAt = trade.UpdatedAt,
            CreatedAt = trade.CreatedAt
        };
    }

    public static TradeDto ToDto(this Trade trade)
    {
        if (trade == null) return null;

        return new TradeDto
        {
            Id = trade.Id,
            ExternalId = trade.ExternalId,
            Symbol = trade.Symbol,
            Direction = trade.Direction,
            EntryPrice = trade.EntryPrice,
            ExitPrice = trade.ExitPrice,
            StopLoss = trade.StopLoss,
            TakeProfit = trade.TakeProfit,
            LotSize = trade.LotSize,
            Profit = trade.Profit,
            ProfitPips = trade.ProfitPips,
            Mae = trade.Mae,
            Mfe = trade.Mfe,
            Efficiency = trade.Efficiency,
            EntryTime = trade.EntryTime,
            ExitTime = trade.ExitTime,
            DurationMinutes = trade.DurationMinutes,
            ChartData = trade.ChartData.ToDto(),
            Status = trade.Status,
            UpdatedAt = trade.UpdatedAt,
            CreatedAt = trade.CreatedAt
        };
    }

    public static ChartDataDto ToDto(this ChartData chartData)
    {
        if (chartData == null) return null;

        return new ChartDataDto
        {
            OhlcDataBefore = chartData.OhlcDataBefore.ToDto(),
            OhlcDataAfter = chartData.OhlcDataAfter.ToDto(),
            ScreenshotUrlBefore = chartData.ScreenshotUrlBefore,
            ScreenshotUrlAfter = chartData.ScreenshotUrlAfter
        };
    }

    public static OhlcDataDto ToDto(this OhlcData ohlcData)
    {
        if (ohlcData == null) return null;

        return new OhlcDataDto
        {
            Timeframe = ohlcData.Timeframe,
            EntryTime = ohlcData.EntryTime,
            ExitTime = ohlcData.ExitTime,
            Candles = ohlcData.Candles?.Select(c => new CandleDto
            {
                Time = c.Time,
                Open = c.Open,
                High = c.High,
                Low = c.Low,
                Close = c.Close,
                Volume = c.Volume
            }).ToList()
        };
    }

    #endregion

    #region Domain

    public static Trade ToDomain(this TradeDto tradeDto)
    {
        if (tradeDto == null) return null;

        return new Trade
        {
            Id = tradeDto.Id,
            ExternalId = tradeDto.ExternalId,
            Symbol = tradeDto.Symbol,
            Direction = tradeDto.Direction,
            EntryPrice = tradeDto.EntryPrice,
            ExitPrice = tradeDto.ExitPrice,
            StopLoss = tradeDto.StopLoss,
            TakeProfit = tradeDto.TakeProfit,
            LotSize = tradeDto.LotSize,
            Profit = tradeDto.Profit,
            ProfitPips = tradeDto.ProfitPips,
            Mae = tradeDto.Mae,
            Mfe = tradeDto.Mfe,
            Efficiency = tradeDto.Efficiency,
            EntryTime = tradeDto.EntryTime,
            ExitTime = tradeDto.ExitTime,
            DurationMinutes = tradeDto.DurationMinutes,
            ChartData = tradeDto.ChartData.ToDomain(),
            Status = tradeDto.Status,
            UpdatedAt = tradeDto.UpdatedAt,
            CreatedAt = tradeDto.CreatedAt
        };
    }

    public static ChartData ToDomain(this ChartDataDto dto)
    {
        if (dto == null) return null;

        return new ChartData
        {
            OhlcDataBefore = dto.OhlcDataBefore.ToDomain(),
            OhlcDataAfter = dto.OhlcDataAfter.ToDomain(),
            ScreenshotUrlBefore = dto.ScreenshotUrlBefore,
            ScreenshotUrlAfter = dto.ScreenshotUrlAfter
        };
    }

    public static OhlcData ToDomain(this OhlcDataDto dto)
    {
        if (dto == null) return null;

        return new OhlcData
        {
            Timeframe = dto.Timeframe,
            EntryTime = dto.EntryTime,
            ExitTime = dto.ExitTime,
            Candles = dto.Candles?.Select(c => new Candle
            {
                Time = c.Time,
                Open = c.Open,
                High = c.High,
                Low = c.Low,
                Close = c.Close,
                Volume = c.Volume
            }).ToList()
        };
    }

    #endregion
}
