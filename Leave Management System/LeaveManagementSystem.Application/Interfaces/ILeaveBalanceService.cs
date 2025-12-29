using LeaveManagementSystem.Application.DTOs;

namespace LeaveManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for leave balance operations.
/// Following Interface Segregation Principle - defines only leave balance-related operations.
/// Following Dependency Inversion Principle - high-level modules depend on abstraction.
/// </summary>
public interface ILeaveBalanceService
{
    /// <summary>
    /// Gets leave balances for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of leave balance DTOs</returns>
    Task<IEnumerable<LeaveBalanceDto>> GetLeaveBalancesByUserIdAsync(Guid userId);

    /// <summary>
    /// Gets a specific leave balance for a user and leave type
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="leaveTypeId">Leave type ID</param>
    /// <returns>Leave balance DTO if found, null otherwise</returns>
    Task<LeaveBalanceDto?> GetLeaveBalanceAsync(Guid userId, Guid leaveTypeId);

    /// <summary>
    /// Gets or creates a leave balance for a user and leave type.
    /// If the balance doesn't exist, it will be created with the default entitlement.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="leaveTypeId">Leave type ID</param>
    /// <param name="defaultUnit">Default unit for the balance if creating new</param>
    /// <returns>Leave balance DTO</returns>
    Task<LeaveBalanceDto> GetOrCreateLeaveBalanceAsync(Guid userId, Guid leaveTypeId, Domain.Enums.LeaveUnit defaultUnit);

    /// <summary>
    /// Updates leave balance after a leave request is approved
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="leaveTypeId">Leave type ID</param>
    /// <param name="durationAmount">Duration amount to deduct</param>
    /// <param name="durationUnit">Duration unit</param>
    Task UpdateLeaveBalanceAsync(Guid userId, Guid leaveTypeId, decimal durationAmount, Domain.Enums.LeaveUnit durationUnit);

    /// <summary>
    /// Adds back to leave balance when a leave request is rejected after being approved
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="leaveTypeId">Leave type ID</param>
    /// <param name="durationAmount">Duration amount to add back</param>
    /// <param name="durationUnit">Duration unit</param>
    Task AddToLeaveBalanceAsync(Guid userId, Guid leaveTypeId, decimal durationAmount, Domain.Enums.LeaveUnit durationUnit);
}

