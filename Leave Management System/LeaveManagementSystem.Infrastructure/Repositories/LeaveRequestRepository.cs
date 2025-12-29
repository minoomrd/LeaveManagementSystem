using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Specialized repository for LeaveRequest operations.
/// Following Single Responsibility Principle - handles only leave request data access.
/// </summary>
public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all leave requests for a user with user and leave type information in a single query.
    /// Uses JOINs to avoid DbContext concurrency issues.
    /// </summary>
    public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdWithDetailsAsync(Guid userId)
    {
        // Single query with explicit JOINs - NO navigation properties, NO lazy loading
        // This completely avoids DbContext concurrency issues
        return await _context.LeaveRequests
            .Join(_context.Users, lr => lr.UserId, u => u.Id, (lr, u) => new { lr, u })
            .Join(_context.LeaveTypes, x => x.lr.LeaveTypeId, lt => lt.Id, (x, lt) => new { x.lr, x.u, lt })
            .Where(x => x.lr.UserId == userId)
            .Select(x => new LeaveRequestDto
            {
                Id = x.lr.Id,
                UserId = x.lr.UserId,
                UserName = x.u.FullName,
                LeaveTypeId = x.lr.LeaveTypeId,
                LeaveTypeName = x.lt.Name,
                StartDateTime = x.lr.StartDateTime,
                EndDateTime = x.lr.EndDateTime,
                DurationAmount = x.lr.DurationAmount,
                DurationUnit = x.lr.DurationUnit,
                Reason = x.lr.Reason,
                Status = x.lr.Status,
                AdminComment = x.lr.AdminComment,
                CreatedAt = x.lr.CreatedAt
            })
            .AsNoTracking()
            .ToListAsync();
    }
}



