using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new leave request.
/// Following Single Responsibility Principle - represents only leave request creation data.
/// </summary>
public class CreateLeaveRequestDto
{
    /// <summary>
    /// Leave type ID
    /// </summary>
    public Guid LeaveTypeId { get; set; }

    /// <summary>
    /// Start date and time of the leave
    /// </summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// End date and time of the leave
    /// </summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Optional reason for the leave
    /// </summary>
    public string? Reason { get; set; }
}

