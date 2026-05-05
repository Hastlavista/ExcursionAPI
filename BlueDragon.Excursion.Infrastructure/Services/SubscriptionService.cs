using System;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Subscription;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Domain.Models;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using Stripe;
using Stripe.BillingPortal;

namespace BlueDragon.Excursion.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IAuthHandler _authHandler;
    private readonly ISubscriptionHandler _subscriptionHandler;
    private readonly StripeSettings _stripeSettings;

    public SubscriptionService(IAuthHandler authHandler, ISubscriptionHandler subscriptionHandler, StripeSettings stripeSettings)
    {
        _authHandler = authHandler;
        _subscriptionHandler = subscriptionHandler;
        _stripeSettings = stripeSettings;
    }

    public async Task<CreateCheckoutSessionResponse> CreateCheckoutSession(Guid userId)
    {
        User user = await _authHandler.GetUserById(userId);
        if (user == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        StripeClient stripeClient = new StripeClient(_stripeSettings.SecretKey);

        string customerId = user.StripeCustomerId;
        if (string.IsNullOrEmpty(customerId))
        {
            CustomerService customerService = new CustomerService(stripeClient);
            Customer customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = user.Email,
                Metadata = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "userId", userId.ToString() }
                }
            });
            customerId = customer.Id;
            await _subscriptionHandler.UpdateStripeCustomerId(userId, customerId);
        }

        Stripe.Checkout.SessionService sessionService = new Stripe.Checkout.SessionService(stripeClient);
        Stripe.Checkout.Session session = await sessionService.CreateAsync(new Stripe.Checkout.SessionCreateOptions
        {
            Customer = customerId,
            Mode = "subscription",
            LineItems =
            [
                new Stripe.Checkout.SessionLineItemOptions
                {
                    Price = _stripeSettings.ProPriceId,
                    Quantity = 1
                }
            ],
            SuccessUrl = _stripeSettings.SuccessUrl,
            CancelUrl = _stripeSettings.CancelUrl
        });

        return new CreateCheckoutSessionResponse { Url = session.Url };
    }

    public async Task<CustomerPortalResponse> CreateCustomerPortal(Guid userId)
    {
        User user = await _authHandler.GetUserById(userId);
        if (user == null)
            throw new ArgumentException($"User with id {userId} does not exist");

        if (string.IsNullOrEmpty(user.StripeCustomerId))
            throw new InvalidOperationException("No Stripe customer found for this user");

        StripeClient stripeClient = new StripeClient(_stripeSettings.SecretKey);
        SessionService portalService = new SessionService(stripeClient);
        Session portalSession = await portalService.CreateAsync(new SessionCreateOptions
        {
            Customer = user.StripeCustomerId,
            ReturnUrl = _stripeSettings.SuccessUrl
        });

        return new CustomerPortalResponse { Url = portalSession.Url };
    }

    public async Task HandleWebhook(string payload, string stripeSignature)
    {
        Event stripeEvent = EventUtility.ConstructEvent(payload, stripeSignature, _stripeSettings.WebhookSecret, throwOnApiVersionMismatch: false);

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
                await HandleCheckoutSessionCompleted(stripeEvent);
                break;
            case EventTypes.CustomerSubscriptionDeleted:
                await HandleSubscriptionDeleted(stripeEvent);
                break;
        }
    }

    private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        Stripe.Checkout.Session session = stripeEvent.Data.Object as Stripe.Checkout.Session;
        if (session == null)
            return;

        string stripeCustomerId = session.CustomerId;
        string stripeSubscriptionId = session.SubscriptionId;

        User user = await _subscriptionHandler.GetUserByStripeCustomerId(stripeCustomerId);
        if (user == null)
            return;

        await _subscriptionHandler.ActivateProSubscription(user.Id.GetValueOrDefault(), stripeCustomerId, stripeSubscriptionId);
    }

    private async Task HandleSubscriptionDeleted(Event stripeEvent)
    {
        Subscription subscription = stripeEvent.Data.Object as Subscription;
        if (subscription == null)
            return;

        await _subscriptionHandler.DeactivateProSubscription(subscription.CustomerId);
    }
}
