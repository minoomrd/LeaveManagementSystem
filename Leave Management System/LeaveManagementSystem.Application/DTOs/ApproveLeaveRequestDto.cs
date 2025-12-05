namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for approving/rejecting a leave request.
/// Following Single Responsibility Principle - represents only approval action data.
/// </summary>
public class ApproveLeaveRequestDto
{
    /// <summary>
    /// Optional comment from the admin
    /// </summary>
    public string? AdminComment { get; set; }
}

