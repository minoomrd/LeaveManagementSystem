using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for User entity.
/// Following Single Responsibility Principle - represents only user data for transfer.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Role ID of the user (foreign key to Role entity)
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Role name (for convenience)
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Status of the user account
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Timestamp when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

