using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlueDragon.Excursion.Infrastructure.Utils;

public static class UserUtils
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        string value = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                       ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out Guid id) ? id : Guid.Empty;
    }
}