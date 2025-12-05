using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents custom leave entitlements for a specific employee.
/// Following Single Responsibility Principle - represents only employee-specific leave settings.
/// Allows overriding default leave policies for individual employees.
/// </summary>
public class EmployeeLeaveSetting : BaseEntity
{
    /// <summary>
    /// Foreign key to the user/employee
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Foreign key to the leave type
    /// </summary>
    public Guid LeaveTypeId { get; set; }

    /// <summary>
    /// Navigation property to the leave type
    /// </summary>
    public virtual LeaveType LeaveType { get; set; } = null!;

    /// <summary>
    /// Custom entitlement amount for this employee (overrides default policy)
    /// </summary>
    public decimal CustomEntitlementAmount { get; set; }

    /// <summary>
    /// Unit of the custom entitlement (Day or Hour)
    /// </summary>
    public LeaveUnit CustomEntitlementUnit { get; set; }
}

