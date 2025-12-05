namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for updating a role.
/// Following Single Responsibility Principle - represents only role update data.
/// </summary>
public class UpdateRoleDto
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

