namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for user login.
/// Following Single Responsibility Principle - represents only login credentials.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Email address for login
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Plain text password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

