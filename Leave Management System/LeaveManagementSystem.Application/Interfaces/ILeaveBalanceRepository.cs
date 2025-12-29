using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Application.Interfaces;

/// <summary>
/// Specialized repository interface for LeaveBalance operations.
/// Following Interface Segregation Principle - provides only leave balance-specific operations.
/// </summary>
public interface ILeaveBalanceRepository : IRepository<LeaveBalance>
{
    /// <summary>
    /// Gets all leave balances for a user with leave type information in a single query.
    /// This method uses JOINs to avoid DbContext concurrency issues.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of leave balance DTOs with leave type information</returns>
    Task<IEnumerable<LeaveBalanceDto>> GetLeaveBalancesByUserIdWithDetailsAsync(Guid userId);
}



