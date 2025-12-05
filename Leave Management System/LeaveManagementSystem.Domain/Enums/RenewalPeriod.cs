namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the period after which leave balance is renewed.
/// Following Single Responsibility Principle - defines only renewal period types.
/// </summary>
public enum RenewalPeriod
{
    /// <summary>
    /// Leave balance renews weekly
    /// </summary>
    Weekly = 1,

    /// <summary>
    /// Leave balance renews monthly
    /// </summary>
    Monthly = 2,

    /// <summary>
    /// Leave balance renews yearly
    /// </summary>
    Yearly = 3
}

