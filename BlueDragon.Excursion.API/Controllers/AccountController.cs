using System.Net;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Auth;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueDragon.Excursion.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegenerateApiKey()
    {
        AuthResponse response = await _accountService.RegenerateApiKey(User.GetUserId());
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        bool success = await _accountService.ChangePassword(User.GetUserId(), request);
        if (!success)
            return BadRequest("Current password is incorrect!");

        return Ok("Password changed successfully!");
    }

    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteAccount()
    {
        await _accountService.DeleteAccount(User.GetUserId());
        return Ok();
    }
}