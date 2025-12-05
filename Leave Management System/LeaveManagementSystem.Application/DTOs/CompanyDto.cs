using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for Company entity.
/// Following Single Responsibility Principle - represents only company data for transfer.
/// </summary>
public class CompanyDto
{
    /// <summary>
    /// Unique identifier of the company
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Company name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact email
    /// </summary>
    public string ContactEmail { get; set; } = string.Empty;

    /// <summary>
    /// Owner ID
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Status of the company
    /// </summary>
    public CompanyStatus Status { get; set; }

    /// <summary>
    /// Total number of users in the company
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Timestamp when the company was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

