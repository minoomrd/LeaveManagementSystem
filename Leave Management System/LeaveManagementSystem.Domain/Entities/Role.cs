using LeaveManagementSystem.Domain.Common;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a role in the system.
/// Following Single Responsibility Principle - represents only role data.
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Name of the role (must be unique)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the role
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the role is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

