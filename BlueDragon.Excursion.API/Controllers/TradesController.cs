using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Trades;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Core.Shared;
using BlueDragon.Excursion.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueDragon.Excursion.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
public class TradesController : Controller
{
    private readonly ITradeService _tradeService;

    public TradesController(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> OpenTrade([FromBody] TradeDto trade)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        await _tradeService.OpenTrade(trade, User.GetUserId());
        return Ok("trade-saved-successfully");
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CloseTrade([FromBody] TradeDto trade)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        await _tradeService.CloseTrade(trade);
        return Ok("Trade successfully closed!");
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateTrade([FromBody] TradeDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        await _tradeService.UpdateTrade(request);
        return Ok("Trade successfully updated!");
    }

    [HttpPost]
    [ProducesResponseType(typeof(List<TradeBaseDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetTrades()
    {
        List<TradeBaseDto> trades = await _tradeService.GetTrades(User.GetUserId());
        return Ok(trades);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TradeDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetTrade([FromBody] GuidIdRequest request)
    {
        if (!ModelState.IsValid || request.Id == null)
            return BadRequest("model-invalid");
        
        TradeDto trade = await _tradeService.GetTrade(request.Id.GetValueOrDefault(), User.GetUserId());
        if (trade == null)
            return NotFound();

        return Ok(trade);
    }
}