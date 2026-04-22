using System;
using System.Net;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Auth;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlueDragon.Excursion.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegenerateApiKey()
    {
        try
        {
            AuthResponse response = await _accountService.RegenerateApiKey(User.GetUserId());
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RegenerateApiKey failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        try
        {
            bool success = await _accountService.ChangePassword(User.GetUserId(), request);
            if (!success)
                return BadRequest("Current password is incorrect!");

            return Ok("Password changed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChangePassword failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteAccount()
    {
        try
        {
            await _accountService.DeleteAccount(User.GetUserId());
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAccount failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PlanResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPlan()
    {
        try
        {
            PlanResponse plan = await _accountService.GetPlan(User.GetUserId());
            return Ok(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPlan failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
