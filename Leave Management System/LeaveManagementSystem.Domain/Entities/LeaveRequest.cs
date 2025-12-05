using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a leave request submitted by an employee.
/// Following Single Responsibility Principle - represents only leave request data.
/// </summary>
public class LeaveRequest : BaseEntity
{
    /// <summary>
    /// Foreign key to the user/employee who submitted the request
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
    /// Start date and time of the leave
    /// </summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// End date and time of the leave
    /// </summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Calculated duration amount (e.g., 2.5 days or 8 hours)
    /// </summary>
    public decimal DurationAmount { get; set; }

    /// <summary>
    /// Unit of the duration (Day or Hour)
    /// </summary>
    public LeaveUnit DurationUnit { get; set; }

    /// <summary>
    /// Optional reason provided by the employee
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Status of the leave request (Pending, Approved, or Rejected)
    /// </summary>
    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

    /// <summary>
    /// Optional comment from the admin who reviewed the request
    /// </summary>
    public string? AdminComment { get; set; }
}

