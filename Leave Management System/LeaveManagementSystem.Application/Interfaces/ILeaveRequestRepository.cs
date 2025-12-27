using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Application.Interfaces;

/// <summary>
/// Specialized repository interface for LeaveRequest operations.
/// Following Interface Segregation Principle - provides only leave request-specific operations.
/// </summary>
public interface ILeaveRequestRepository : IRepository<LeaveRequest>
{
    /// <summary>
    /// Gets all leave requests for a user with user and leave type information in a single query.
    /// This method uses JOINs to avoid DbContext concurrency issues.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of leave request DTOs with user and leave type information</returns>
    Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdWithDetailsAsync(Guid userId);
}

