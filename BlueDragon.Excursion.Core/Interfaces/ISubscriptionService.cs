using System;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Subscription;

namespace BlueDragon.Excursion.Core.Interfaces;

public interface ISubscriptionService
{
    Task<CreateCheckoutSessionResponse> CreateCheckoutSession(Guid userId);
    Task<CustomerPortalResponse> CreateCustomerPortal(Guid userId);
    Task HandleWebhook(string payload, string stripeSignature);
}
