namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new user.
/// Following Single Responsibility Principle - represents only user creation data.
/// </summary>
public class CreateUserDto
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
    /// Plain text password (will be hashed)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Role ID of the user (foreign key to Role entity)
    /// </summary>
    public Guid RoleId { get; set; }
}

