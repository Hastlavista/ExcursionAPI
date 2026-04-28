using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueDragon.Excursion.Infrastructure.Domain.Models;

[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid? Id { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; }

    [Column("api_key")]
    public string ApiKey { get; set; }

    [Column("is_pro")]
    public bool? IsPro { get; set; }

    [Column("trades_this_month")]
    public int TradesThisMonth { get; set; }

    [Column("trades_reset_date")]
    public DateTimeOffset TradesResetDate { get; set; }

    [Column("stripe_customer_id")]
    public string StripeCustomerId { get; set; }

    [Column("stripe_subscription_id")]
    public string StripeSubscriptionId { get; set; }

    [Column("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }
}
