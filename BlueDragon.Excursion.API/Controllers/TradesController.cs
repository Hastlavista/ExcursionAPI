using System;
using System.Net;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Trades;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Core.Shared;
using BlueDragon.Excursion.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlueDragon.Excursion.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
public class TradesController : Controller
{
    private readonly ITradeService _tradeService;
    private readonly ILogger<TradesController> _logger;

    public TradesController(ITradeService tradeService, ILogger<TradesController> logger)
    {
        _tradeService = tradeService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> OpenTrade([FromBody] TradeDto trade)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        try
        {
            await _tradeService.OpenTrade(trade, User.GetUserId());
            return Ok("trade-saved-successfully");
        }
        catch (TradeLimitExceededException ex)
        {
            return StatusCode(StatusCodes.Status402PaymentRequired, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenTrade failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CloseTrade([FromBody] TradeDto trade)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        if (trade.Id == null && trade.ExternalId == null)
            return BadRequest("Id or ExternalId is required");

        try
        {
            await _tradeService.CloseTrade(trade);
            return Ok("Trade successfully closed!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CloseTrade failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateTrade([FromBody] TradeDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        if (request.ExternalId == null)
            return BadRequest("ExternalId is required");

        try
        {
            await _tradeService.UpdateTrade(request);
            return Ok("Trade successfully updated!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateTrade failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
