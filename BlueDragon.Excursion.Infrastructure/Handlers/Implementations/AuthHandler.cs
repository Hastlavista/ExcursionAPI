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
}
