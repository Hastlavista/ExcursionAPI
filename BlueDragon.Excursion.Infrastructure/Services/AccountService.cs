using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Auth;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;

namespace BlueDragon.Excursion.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly IAuthHandler _authHandler;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AccountService(IAuthHandler authHandler, IJwtService jwtService, JwtSettings jwtSettings)
    {
        _authHandler = authHandler;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponse> RegenerateApiKey(Guid userId)
    {
        User user = await _authHandler.GetUserById(userId);
        if (user == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        string newApiKey = Guid.NewGuid().ToString("N");
        await _authHandler.UpdateApiKey(userId, newApiKey);

        user.ApiKey = newApiKey;
        return ToAuthResponse(user);
    }

    public async Task<bool> ChangePassword(Guid userId, ChangePasswordRequest request)
    {
        User user = await _authHandler.GetUserById(userId);
        if (user == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        string currentHash = HashPassword(request.CurrentPassword);
        if (user.PasswordHash != currentHash)
            return false;

        string newHash = HashPassword(request.NewPassword);
        await _authHandler.UpdatePasswordHash(userId, newHash);
        return true;
    }

    public async Task DeleteAccount(Guid userId)
    {
        await _authHandler.DeleteUserWithTrades(userId);
    }

    public async Task<PlanResponse> GetPlan(Guid userId)
    {
        User user = await _authHandler.GetUserById(userId);
        if (user == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        return new PlanResponse
        {
            IsPro = user.IsPro ?? false,
            TradesThisMonth = user.TradesThisMonth,
            TradeLimit = 25
        };
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