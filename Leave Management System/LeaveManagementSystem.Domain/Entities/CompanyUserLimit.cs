using LeaveManagementSystem.Domain.Common;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents the free user limit configuration for a company.
/// Following Single Responsibility Principle - represents only company user limit data.
/// </summary>
public class CompanyUserLimit : BaseEntity
{
    /// <summary>
    /// Foreign key to the company
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Navigation property to the company
    /// </summary>
    public virtual Company Company { get; set; } = null!;

    /// <summary>
    /// Number of free users allowed (e.g., 5 users free)
    /// </summary>
    public int FreeUserLimit { get; set; } = 5;
}

