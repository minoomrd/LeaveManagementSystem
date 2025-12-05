namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the status of a payment transaction.
/// Following Single Responsibility Principle - defines only payment status values.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment was successful
    /// </summary>
    Success = 1,

    /// <summary>
    /// Payment failed
    /// </summary>
    Failed = 2
}

