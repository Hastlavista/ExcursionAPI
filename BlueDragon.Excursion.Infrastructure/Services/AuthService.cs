using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Auth;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;

namespace BlueDragon.Excursion.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IAuthHandler _authHandler;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IAuthHandler authHandler, IJwtService jwtService, JwtSettings jwtSettings)
    {
        _authHandler = authHandler;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        bool emailExists = await _authHandler.EmailExists(request.Email);
        if (emailExists)
            return null;

        User user = new User();
        user.Id = Guid.NewGuid();
        user.Email = request.Email;
        user.PasswordHash = HashPassword(request.Password);
        user.ApiKey = Guid.NewGuid().ToString("N");
        user.IsPro = false;
        user.CreatedAt = DateTimeOffset.UtcNow;

        await _authHandler.AddUser(user);

        return ToAuthResponse(user);
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        string passwordHash = HashPassword(request.Password);
        User user = await _authHandler.GetUserByCredentials(request.Email, passwordHash);

        if (user == null)
            return null;

        return ToAuthResponse(user);
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }

    private AuthResponse ToAuthResponse(User user)
    {
        string token = _jwtService.GenerateToken(user.Id.GetValueOrDefault(), user.Email);

        AuthResponse response = new AuthResponse();
        response.UserId = user.Id;
        response.Email = user.Email;
        response.ApiKey = user.ApiKey;
        response.IsPro = user.IsPro;
        response.Token = token;
        response.TokenExpiration = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);
        return response;
    }
}
