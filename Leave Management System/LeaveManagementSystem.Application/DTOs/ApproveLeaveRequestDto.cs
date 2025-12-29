namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for approving/rejecting a leave request (for backward compatibility).
/// Following Single Responsibility Principle - represents only approval/rejection action data.
/// </summary>
public class ApproveLeaveRequestDto
{
    /// <summary>
    /// Optional comment from the admin
    /// </summary>
    public string? AdminComment { get; set; }
}
