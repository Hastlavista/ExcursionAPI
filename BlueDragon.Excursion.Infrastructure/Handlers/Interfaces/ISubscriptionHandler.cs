using System;
using System.Threading.Tasks;
using BlueDragon.Excursion.Infrastructure.Domain.Models;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;

public interface ISubscriptionHandler
{
    Task<User> GetUserByStripeCustomerId(string stripeCustomerId);
    Task UpdateStripeCustomerId(Guid userId, string stripeCustomerId);
    Task ActivateProSubscription(Guid userId, string stripeCustomerId, string stripeSubscriptionId);
    Task DeactivateProSubscription(string stripeCustomerId);
}
