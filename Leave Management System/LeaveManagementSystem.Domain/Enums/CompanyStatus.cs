namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the status of a company account.
/// Following Single Responsibility Principle - defines only company status values.
/// </summary>
public enum CompanyStatus
{
    /// <summary>
    /// Company account is active
    /// </summary>
    Active = 1,

    /// <summary>
    /// Company account is suspended
    /// </summary>
    Suspended = 2
}

