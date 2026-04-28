using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Subscription;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;

namespace BlueDragon.Excursion.API.Controllers;

[Route("api/[controller]/[action]")]
[Produces("application/json")]
public class SubscriptionController : Controller
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(ISubscriptionService subscriptionService, ILogger<SubscriptionController> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(CreateCheckoutSessionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        try
        {
            CreateCheckoutSessionResponse response = await _subscriptionService.CreateCheckoutSession(User.GetUserId());
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateCheckoutSession failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(CustomerPortalResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CustomerPortal()
    {
        try
        {
            CustomerPortalResponse response = await _subscriptionService.CreateCustomerPortal(User.GetUserId());
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CustomerPortal failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Webhook()
    {
        string payload;
        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            payload = await reader.ReadToEndAsync();
        }

        string stripeSignature = Request.Headers["Stripe-Signature"];

        try
        {
            await _subscriptionService.HandleWebhook(payload, stripeSignature);
            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "Stripe webhook validation failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook handling failed: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
