namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new role.
/// Following Single Responsibility Principle - represents only role creation data.
/// </summary>
public class CreateRoleDto
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

