namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the billing period for company subscriptions.
/// Following Single Responsibility Principle - defines only billing period types.
/// </summary>
public enum BillingPeriod
{
    /// <summary>
    /// Monthly billing cycle
    /// </summary>
    Monthly = 1,

    /// <summary>
    /// Yearly billing cycle
    /// </summary>
    Yearly = 2
}

