using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Specialized repository for LeaveBalance operations.
/// Following Single Responsibility Principle - handles only leave balance data access.
/// </summary>
public class LeaveBalanceRepository : Repository<LeaveBalance>, ILeaveBalanceRepository
{
    public LeaveBalanceRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all leave balances for a user with leave type information in a single query.
    /// Uses JOINs to avoid DbContext concurrency issues.
    /// </summary>
    public async Task<IEnumerable<LeaveBalanceDto>> GetLeaveBalancesByUserIdWithDetailsAsync(Guid userId)
    {
        // Single query with explicit JOIN - NO navigation properties, NO lazy loading
        // This completely avoids DbContext concurrency issues
        return await _context.LeaveBalances
            .Join(_context.LeaveTypes, lb => lb.LeaveTypeId, lt => lt.Id, (lb, lt) => new { lb, lt })
            .Where(x => x.lb.UserId == userId)
            .Select(x => new LeaveBalanceDto
            {
                Id = x.lb.Id,
                UserId = x.lb.UserId,
                LeaveTypeId = x.lb.LeaveTypeId,
                LeaveTypeName = x.lt.Name,
                BalanceAmount = x.lb.BalanceAmount,
                BalanceUnit = x.lb.BalanceUnit,
                UpdatedAt = x.lb.UpdatedAt
            })
            .AsNoTracking()
            .ToListAsync();
    }
}



