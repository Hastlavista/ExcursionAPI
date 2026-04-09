using System.Net;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Auth;
using BlueDragon.Excursion.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlueDragon.Excursion.API.Controllers;

[Route("api/public/[controller]/[action]")]
[Produces("application/json")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        AuthResponse response = await _authService.Register(request);
        if (response == null)
            return Conflict("Email already in use!");

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("model-invalid");

        AuthResponse response = await _authService.Login(request);
        if (response == null)
            return Unauthorized("Invalid email or password!");

        return Ok(response);
    }
}
