using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Entities;

/// <summary>
/// Represents a company/organization using the platform.
/// Following Single Responsibility Principle - represents only company data.
/// </summary>
public class Company : BaseEntity
{
    /// <summary>
    /// Name of the company
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact email for the company
    /// </summary>
    public string ContactEmail { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the owner/admin user
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Navigation property to the owner user
    /// </summary>
    public virtual User Owner { get; set; } = null!;

    /// <summary>
    /// Status of the company (Active or Suspended)
    /// </summary>
    public CompanyStatus Status { get; set; } = CompanyStatus.Active;

    /// <summary>
    /// Company user limit configuration
    /// </summary>
    public virtual CompanyUserLimit? CompanyUserLimit { get; set; }

    /// <summary>
    /// Company subscription information
    /// </summary>
    public virtual CompanySubscription? CompanySubscription { get; set; }

    /// <summary>
    /// Collection of payments made by this company
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

