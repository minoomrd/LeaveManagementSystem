namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the unit of measurement for leave (hourly or daily).
/// Following Single Responsibility Principle - defines only leave unit types.
/// </summary>
public enum LeaveUnit
{
    /// <summary>
    /// Leave is measured in hours
    /// </summary>
    Hour = 1,

    /// <summary>
    /// Leave is measured in days
    /// </summary>
    Day = 2
}

