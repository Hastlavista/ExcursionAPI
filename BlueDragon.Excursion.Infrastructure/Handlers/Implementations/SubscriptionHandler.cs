using System;
using System.Threading.Tasks;
using BlueDragon.Excursion.Infrastructure.Domain.Contexts;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Implementations;

public class SubscriptionHandler : ISubscriptionHandler
{
    private readonly DatabaseSettings _databaseSettings;

    public SubscriptionHandler(DatabaseSettings databaseSettings)
    {
        _databaseSettings = databaseSettings;
    }

    public async Task<User> GetUserByStripeCustomerId(string stripeCustomerId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        return await context.Users.SingleOrDefaultAsync(u => u.StripeCustomerId == stripeCustomerId);
    }

    public async Task UpdateStripeCustomerId(Guid userId, string stripeCustomerId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        User existing = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (existing == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        existing.StripeCustomerId = stripeCustomerId;
        context.Users.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task ActivateProSubscription(Guid userId, string stripeCustomerId, string stripeSubscriptionId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        User existing = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (existing == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        existing.IsPro = true;
        existing.StripeCustomerId = stripeCustomerId;
        existing.StripeSubscriptionId = stripeSubscriptionId;
        context.Users.Update(existing);
        await context.SaveChangesAsync();
    }

    public async Task DeactivateProSubscription(string stripeCustomerId)
    {
        await using DatabaseContext context = DatabaseContext.GenerateContext(_databaseSettings.ConnectionString);
        User existing = await context.Users.SingleOrDefaultAsync(u => u.StripeCustomerId == stripeCustomerId);
        if (existing == null)
            return;

        existing.IsPro = false;
        existing.StripeSubscriptionId = null;
        context.Users.Update(existing);
        await context.SaveChangesAsync();
    }
}
