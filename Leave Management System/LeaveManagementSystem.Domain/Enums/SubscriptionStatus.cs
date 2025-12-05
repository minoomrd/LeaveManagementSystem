namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the status of a company subscription.
/// Following Single Responsibility Principle - defines only subscription status values.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Subscription is active and paid
    /// </summary>
    Active = 1,

    /// <summary>
    /// Subscription payment is unpaid
    /// </summary>
    Unpaid = 2,

    /// <summary>
    /// Subscription has been canceled
    /// </summary>
    Canceled = 3
}

