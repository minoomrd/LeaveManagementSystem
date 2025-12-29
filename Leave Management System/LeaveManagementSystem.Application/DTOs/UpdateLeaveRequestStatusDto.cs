using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for updating leave request status.
/// Following Single Responsibility Principle - represents only status update data.
/// </summary>
public class UpdateLeaveRequestStatusDto
{
    /// <summary>
    /// Status to set (Approved or Rejected)
    /// </summary>
    public LeaveRequestStatus Status { get; set; }

    /// <summary>
    /// Optional comment from the admin
    /// </summary>
    public string? AdminComment { get; set; }
}
