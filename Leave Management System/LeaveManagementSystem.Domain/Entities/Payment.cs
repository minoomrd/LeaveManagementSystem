using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a payment transaction for a company.
/// Following Single Responsibility Principle - represents only payment data.
/// </summary>
public class Payment : BaseEntity
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
    /// Payment amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code (e.g., "EUR", "USD")
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Billing period that this payment covers
    /// </summary>
    public string BillingPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the payment was made
    /// </summary>
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Payment method used (e.g., "Stripe", "Credit Card", "Bank Transfer")
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Status of the payment (Success or Failed)
    /// </summary>
    public PaymentStatus Status { get; set; }
}

