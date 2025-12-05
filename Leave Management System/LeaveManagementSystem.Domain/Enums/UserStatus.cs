namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the status of a user account.
/// Following Single Responsibility Principle - defines only user status values.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account is active and can use the system
    /// </summary>
    Active = 1,

    /// <summary>
    /// User account is inactive and cannot access the system
    /// </summary>
    Inactive = 2
}

