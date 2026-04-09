using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlueDragon.Excursion.API.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyHeader = "X-Api-Key";
    private readonly IAuthHandler _authHandler;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IAuthHandler authHandler) : base(options, logger, encoder)
    {
        _authHandler = authHandler;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeader, out var apiKeyValue))
            return AuthenticateResult.NoResult();

        User user = await _authHandler.GetUserByApiKey(apiKeyValue.ToString());
        if (user == null)
            return AuthenticateResult.Fail("Invalid API key");

        Claim[] claims = [new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())];
        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
