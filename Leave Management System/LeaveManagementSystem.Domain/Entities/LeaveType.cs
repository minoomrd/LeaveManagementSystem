using LeaveManagementSystem.Domain.Common;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a type of leave (e.g., Annual Leave, Sick Leave).
/// Following Single Responsibility Principle - represents only leave type data.
/// </summary>
public class LeaveType : BaseEntity
{
    /// <summary>
    /// Name of the leave type (e.g., "Annual Leave", "Sick Leave")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unit of measurement for this leave type (Hour or Day)
    /// </summary>
    public Domain.Enums.LeaveUnit Unit { get; set; }

    /// <summary>
    /// Optional description of the leave type
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if this is a sick leave type
    /// </summary>
    public bool IsSickLeave { get; set; } = false;

    /// <summary>
    /// Collection of leave policies associated with this leave type
    /// </summary>
    public virtual ICollection<LeavePolicy> LeavePolicies { get; set; } = new List<LeavePolicy>();

    /// <summary>
    /// Collection of leave requests of this type
    /// </summary>
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    /// <summary>
    /// Collection of employee leave settings for this leave type
    /// </summary>
    public virtual ICollection<EmployeeLeaveSetting> EmployeeLeaveSettings { get; set; } = new List<EmployeeLeaveSetting>();

    /// <summary>
    /// Collection of leave balances for this leave type
    /// </summary>
    public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}

