using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents working hours configuration for an employee.
/// Following Single Responsibility Principle - represents only working hours data.
/// </summary>
public class WorkingHours : BaseEntity
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
    /// Type of working hours period (Daily, Weekly, Monthly, or Yearly)
    /// </summary>
    public WorkingHoursType Type { get; set; }

    /// <summary>
    /// Number of hours for the specified period
    /// </summary>
    public decimal Hours { get; set; }
}

