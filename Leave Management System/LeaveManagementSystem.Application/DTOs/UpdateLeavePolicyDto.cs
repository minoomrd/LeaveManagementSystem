using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for updating a leave policy.
/// Following Single Responsibility Principle - represents only leave policy update data.
/// </summary>
public class UpdateLeavePolicyDto
{
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

