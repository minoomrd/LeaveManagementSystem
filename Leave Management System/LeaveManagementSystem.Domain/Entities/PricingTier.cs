using LeaveManagementSystem.Domain.Common;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a pricing tier based on user count ranges.
/// Following Single Responsibility Principle - represents only pricing tier data.
/// </summary>
public class PricingTier : BaseEntity
{
    /// <summary>
    /// Minimum number of users for this tier (inclusive)
    /// </summary>
    public int MinUsers { get; set; }

    /// <summary>
    /// Maximum number of users for this tier (inclusive, null for unlimited)
    /// </summary>
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Price per user in this tier
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Currency code (e.g., "EUR", "USD")
    /// </summary>
    public string Currency { get; set; } = "EUR";
}

