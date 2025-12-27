using System.ComponentModel.DataAnnotations;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new leave request.
/// Following Single Responsibility Principle - represents only leave request creation data.
/// </summary>
public class CreateLeaveRequestDto
{
    /// <summary>
    /// Leave type unit (1 = Hour, 2 = Day). Will be used to find the leave type.
    /// </summary>
    [Required]
    public LeaveUnit LeaveTypeUnit { get; set; }

    /// <summary>
    /// Start date and time of the leave (ISO 8601 format: yyyy-MM-ddTHH:mm:ss.fffZ)
    /// </summary>
    [Required]
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// End date and time of the leave (ISO 8601 format: yyyy-MM-ddTHH:mm:ss.fffZ)
    /// </summary>
    [Required]
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Optional reason for the leave
    /// </summary>
    public string? Reason { get; set; }
}

