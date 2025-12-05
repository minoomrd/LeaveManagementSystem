namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the type of working hours period.
/// Following Single Responsibility Principle - defines only working hours period types.
/// </summary>
public enum WorkingHoursType
{
    /// <summary>
    /// Daily working hours
    /// </summary>
    Daily = 1,

    /// <summary>
    /// Weekly working hours
    /// </summary>
    Weekly = 2,

    /// <summary>
    /// Monthly working hours
    /// </summary>
    Monthly = 3,

    /// <summary>
    /// Yearly working hours
    /// </summary>
    Yearly = 4
}

