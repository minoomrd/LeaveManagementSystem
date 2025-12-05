namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the role of a user in the system.
/// Following Single Responsibility Principle - defines only user roles.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular employee who can request leaves
    /// </summary>
    Employee = 1,

    /// <summary>
    /// Company administrator who manages employees and approves leaves
    /// </summary>
    Admin = 2,

    /// <summary>
    /// Platform super-admin who manages companies and billing
    /// </summary>
    SuperAdmin = 3
}

