namespace BlueDragon.Excursion.Infrastructure.Domain.Settings;

public class StripeSettings
{
    public string SecretKey { get; set; }
    public string WebhookSecret { get; set; }
    public string ProPriceId { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
}
