using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a company's subscription to the platform.
/// Following Single Responsibility Principle - represents only subscription data.
/// </summary>
public class CompanySubscription : BaseEntity
{
    /// <summary>
    /// Foreign key to the company
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Navigation property to the company
    /// </summary>
    public virtual Company Company { get; set; } = null!;

    /// <summary>
    /// Billing period (Monthly or Yearly)
    /// </summary>
    public BillingPeriod BillingPeriod { get; set; }

    /// <summary>
    /// Foreign key to the current pricing tier
    /// </summary>
    public Guid? CurrentTierId { get; set; }

    /// <summary>
    /// Navigation property to the pricing tier
    /// </summary>
    public virtual PricingTier? CurrentTier { get; set; }

    /// <summary>
    /// Next billing date
    /// </summary>
    public DateTime NextBillingDate { get; set; }

    /// <summary>
    /// Amount to be charged on next billing
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Status of the subscription (Active, Unpaid, or Canceled)
    /// </summary>
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
}

