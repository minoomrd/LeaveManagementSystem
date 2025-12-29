namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for reviewing a leave request.
/// Following Single Responsibility Principle - represents only review data.
/// </summary>
public class ReviewLeaveRequestDto
{
    /// <summary>
    /// Status to set - "accept" or "reject" (case-insensitive)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Optional comment from the admin
    /// </summary>
    public string? AdminComment { get; set; }
}

