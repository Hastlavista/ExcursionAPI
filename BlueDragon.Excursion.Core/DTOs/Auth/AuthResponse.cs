using System;

namespace BlueDragon.Excursion.Core.DTOs.Auth;

public class AuthResponse
{
    public Guid? UserId { get; set; }
    public string Email { get; set; }
    public string ApiKey { get; set; }
    public bool? IsPro { get; set; }
    public string Token { get; set; }
    public DateTime? TokenExpiration { get; set; }
}
