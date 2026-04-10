using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueDragon.Excursion.Infrastructure.Domain.Contexts;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Implementations;

public class AuthHandler : IAuthHandler
{
    private readonly DatabaseSettings _databaseSettings;

    public AuthHandler(DatabaseSettings databaseSettings)
    {
        _databaseSettings = databaseSettings;
    }

    public async Task<bool> EmailExists(string email)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task AddUser(User user)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetUserByCredentials(string email, string passwordHash)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Users.SingleOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);
    }

    public async Task<User> GetUserByApiKey(string apiKey)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Users.SingleOrDefaultAsync(u => u.ApiKey == apiKey);
    }

    public async Task<User> GetUserById(Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
    }

    public async Task UpdateApiKey(Guid userId, string apiKey)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        User existing = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (existing == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        existing.ApiKey = apiKey;
        context.Users.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task UpdatePasswordHash(Guid userId, string passwordHash)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        User existing = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (existing == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        existing.PasswordHash = passwordHash;
        context.Users.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUserWithTrades(Guid userId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        User existing = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (existing == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        List<Trade> trades = await context.Trades.Where(t => t.UserId == userId).ToListAsync();
        context.Trades.RemoveRange(trades);
        context.Users.Remove(existing);
        await context.SaveChangesAsync();
    }
}
