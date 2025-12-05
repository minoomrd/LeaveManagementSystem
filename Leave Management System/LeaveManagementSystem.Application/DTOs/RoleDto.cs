namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for Role entity.
/// Following Single Responsibility Principle - represents only role data for transfer.
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Unique identifier of the role
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the role
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the role
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the role is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Timestamp when the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the role was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

