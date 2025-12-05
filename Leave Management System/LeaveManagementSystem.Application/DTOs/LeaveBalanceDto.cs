using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.DTOs;

/// <summary>
/// Data Transfer Object for Leave Balance entity.
/// Following Single Responsibility Principle - represents only leave balance data for transfer.
/// </summary>
public class LeaveBalanceDto
{
    /// <summary>
    /// Unique identifier of the balance record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Leave type ID
    /// </summary>
    public Guid LeaveTypeId { get; set; }

    /// <summary>
    /// Leave type name
    /// </summary>
    public string LeaveTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Current balance amount
    /// </summary>
    public decimal BalanceAmount { get; set; }

    /// <summary>
    /// Balance unit
    /// </summary>
    public LeaveUnit BalanceUnit { get; set; }

    /// <summary>
    /// Timestamp when the balance was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

