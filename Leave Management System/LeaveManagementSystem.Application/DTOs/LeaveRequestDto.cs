using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for Leave Request entity.
/// Following Single Responsibility Principle - represents only leave request data for transfer.
/// </summary>
public class LeaveRequestDto
{
    /// <summary>
    /// Unique identifier of the leave request
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID who submitted the request
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User name who submitted the request
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Leave type ID
    /// </summary>
    public Guid LeaveTypeId { get; set; }

    /// <summary>
    /// Leave type name
    /// </summary>
    public string LeaveTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Start date and time of the leave
    /// </summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// End date and time of the leave
    /// </summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Duration amount
    /// </summary>
    public decimal DurationAmount { get; set; }

    /// <summary>
    /// Duration unit
    /// </summary>
    public LeaveUnit DurationUnit { get; set; }

    /// <summary>
    /// Reason for the leave
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Status of the request
    /// </summary>
    public LeaveRequestStatus Status { get; set; }

    /// <summary>
    /// Admin comment
    /// </summary>
    public string? AdminComment { get; set; }

    /// <summary>
    /// Timestamp when the request was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

