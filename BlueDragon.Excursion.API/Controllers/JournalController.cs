using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Requests;
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
public class JournalController : Controller
{
    private readonly IJournalService _journalService;
    private readonly ILogger<JournalController> _logger;

    public JournalController(IJournalService journalService, ILogger<JournalController> logger)
    {
        _journalService = journalService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TradeBaseDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetTrades()
    {
        try
        {
            List<TradeBaseDto> trades = await _journalService.GetTrades(User.GetUserId());
            return Ok(trades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTrades failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(TradeDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetTrade([FromBody] GuidIdRequest request)
    {
        if (!ModelState.IsValid || request.Id == null)
            return BadRequest("model-invalid");

        try
        {
            TradeDto trade = await _journalService.GetTrade(request.Id.GetValueOrDefault(), User.GetUserId());
            if (trade == null)
                return NotFound();

            return Ok(trade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTrade failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CreateTrade([FromBody] TradeDto trade)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        try
        {
            await _journalService.CreateTrade(trade, User.GetUserId());
            return Ok("trade-saved-successfully");
        }
        catch (TradeLimitExceededException ex)
        {
            return StatusCode(StatusCodes.Status402PaymentRequired, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateTrade failed: {Message}", ex.Message);
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

        if (request.Id == null)
            return BadRequest("Id is required");

        try
        {
            await _journalService.UpdateTrade(request, User.GetUserId());
            return Ok("Trade successfully updated!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateTrade failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateScreenshots([FromBody] UpdateScreenshotsRequest request)
    {
        if (!ModelState.IsValid || request.Id == null)
            return BadRequest("model-invalid");

        try
        {
            await _journalService.UpdateScreenshots(request, User.GetUserId());
            return Ok("Screenshot successfully updated!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateScreenshots failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> DeleteTrade([FromBody] GuidIdRequest request)
    {
        if (!ModelState.IsValid || request.Id == null)
            return BadRequest("model-invalid");

        try
        {
            await _journalService.DeleteTrade(request.Id.GetValueOrDefault(), User.GetUserId());
            return Ok("trade-deleted-successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteTrade failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
