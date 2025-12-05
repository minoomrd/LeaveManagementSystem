using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a leave policy that defines entitlements for a leave type.
/// Following Single Responsibility Principle - represents only leave policy data.
/// </summary>
public class LeavePolicy : BaseEntity
{
    /// <summary>
    /// Foreign key to the leave type this policy applies to
    /// </summary>
    public Guid LeaveTypeId { get; set; }

    /// <summary>
    /// Navigation property to the leave type
    /// </summary>
    public virtual LeaveType LeaveType { get; set; } = null!;

    /// <summary>
    /// Amount of leave entitlement (e.g., 2 days, 20 hours)
    /// </summary>
    public decimal EntitlementAmount { get; set; }

    /// <summary>
    /// Unit of the entitlement amount (Day or Hour)
    /// </summary>
    public LeaveUnit EntitlementUnit { get; set; }

    /// <summary>
    /// Period after which the leave balance is renewed (Weekly, Monthly, or Yearly)
    /// </summary>
    public RenewalPeriod RenewalPeriod { get; set; }
}

