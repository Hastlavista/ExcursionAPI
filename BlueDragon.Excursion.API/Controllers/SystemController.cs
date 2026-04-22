using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueDragon.Excursion.API.Controllers;

[AllowAnonymous]
[Route("[controller]")]
public class SystemController : Controller
{
    [HttpGet("/health")]
    public IActionResult Health() => Ok("healthy");
}
