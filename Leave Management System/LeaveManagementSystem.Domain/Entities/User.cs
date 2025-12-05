using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a user in the system (Employee, Admin, or Super-Admin).
/// Following Single Responsibility Principle - represents only user data.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email address (must be unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password for authentication
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the Role entity
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Navigation property to the Role entity
    /// </summary>
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Status of the user account (Active or Inactive)
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>
    /// Collection of leave requests made by this user
    /// </summary>
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    /// <summary>
    /// Collection of leave balances for this user
    /// </summary>
    public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();

    /// <summary>
    /// Collection of employee leave settings for this user
    /// </summary>
    public virtual ICollection<EmployeeLeaveSetting> EmployeeLeaveSettings { get; set; } = new List<EmployeeLeaveSetting>();

    /// <summary>
    /// Collection of working hours configurations for this user
    /// </summary>
    public virtual ICollection<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
}

