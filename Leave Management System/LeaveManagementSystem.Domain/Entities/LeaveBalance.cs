using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents the current leave balance for a user and leave type.
/// Following Single Responsibility Principle - represents only leave balance data.
/// This entity is automatically updated when leave requests are approved.
/// </summary>
public class LeaveBalance : BaseEntity
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
    /// Current remaining balance amount (e.g., 5 days or 40 hours)
    /// </summary>
    public decimal BalanceAmount { get; set; }

    /// <summary>
    /// Unit of the balance (Day or Hour)
    /// </summary>
    public LeaveUnit BalanceUnit { get; set; }

    // Note: UpdatedAt is inherited from BaseEntity
}

