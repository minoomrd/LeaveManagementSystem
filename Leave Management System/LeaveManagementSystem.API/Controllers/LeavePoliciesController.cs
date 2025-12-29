using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Infrastructure.Data;
using LeaveManagementSystem.API.Models;

namespace LeaveManagementSystem.API.Controllers;

/// <summary>
/// Controller for leave policy operations.
/// Following Single Responsibility Principle - handles only leave policy-related HTTP requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeavePoliciesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the LeavePoliciesController class.
    /// </summary>
    /// <param name="context">Database context</param>
    public LeavePoliciesController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all leave policies.
    /// </summary>
    /// <returns>Collection of leave policies</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAllLeavePolicies()
    {
        try
        {
            // Check database connection
            if (!await _context.Database.CanConnectAsync())
            {
                return StatusCode(503, new { error = "Database not available", message = "PostgreSQL is not running or not installed." });
            }

            var policies = await _context.LeavePolicies
                .Include(lp => lp.LeaveType)
                .Select(lp => new
                {
                    lp.Id,
                    lp.LeaveTypeId,
                    LeaveTypeName = lp.LeaveType.Name,
                    lp.LeaveType.IsSickLeave,
                    lp.EntitlementAmount,
                    lp.EntitlementUnit,
                    RenewalPeriod = lp.RenewalPeriod.ToString(), // Convert enum to string
                    lp.CreatedAt,
                    lp.UpdatedAt
                })
                .ToListAsync();

            // Return empty array if no policies exist (this is valid)
            return Ok(policies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", message = ex.Message });
        }
    }

    /// <summary>
    /// Gets leave policies by leave type ID.
    /// </summary>
    /// <param name="leaveTypeId">Leave type ID</param>
    /// <returns>Collection of leave policies</returns>
    [HttpGet("leave-type/{leaveTypeId}")]
    public async Task<ActionResult<IEnumerable<object>>> GetLeavePoliciesByLeaveTypeId(Guid leaveTypeId)
    {
        try
        {
            var policies = await _context.LeavePolicies
                .Include(lp => lp.LeaveType)
                .Where(lp => lp.LeaveTypeId == leaveTypeId)
                .Select(lp => new
                {
                    lp.Id,
                    lp.LeaveTypeId,
                    LeaveTypeName = lp.LeaveType.Name,
                    lp.LeaveType.IsSickLeave,
                    lp.EntitlementAmount,
                    lp.EntitlementUnit,
                    RenewalPeriod = lp.RenewalPeriod.ToString(), // Convert enum to string
                    lp.CreatedAt,
                    lp.UpdatedAt
                })
                .ToListAsync();

            return Ok(policies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", message = ex.Message });
        }
    }

    /// <summary>
    /// Creates or updates a leave policy for a leave type.
    /// If a policy already exists for the leave type, it updates it; otherwise, creates a new one.
    /// </summary>
    /// <param name="leaveTypeId">Leave type ID</param>
    /// <param name="createDto">Leave policy creation data</param>
    /// <returns>Created or updated leave policy</returns>
    [HttpPost("leave-type/{leaveTypeId}")]
    public async Task<ActionResult<object>> CreateOrUpdateLeavePolicy(Guid leaveTypeId, [FromBody] CreateLeavePolicyRequest request)
    {
        try
        {
            // Check database connection
            if (!await _context.Database.CanConnectAsync())
            {
                return StatusCode(503, new { error = "Database not available", message = "PostgreSQL is not running or not installed." });
            }

            // Parse RenewalPeriod from string
            if (!Enum.TryParse<RenewalPeriod>(request.RenewalPeriod, true, out var renewalPeriod))
            {
                return BadRequest(new { error = $"Invalid renewal period: {request.RenewalPeriod}. Must be Weekly, Monthly, or Yearly." });
            }

            // Convert to DTO
            var createDto = new CreateLeavePolicyDto
            {
                EntitlementAmount = request.EntitlementAmount,
                EntitlementUnit = (LeaveUnit)request.EntitlementUnit,
                RenewalPeriod = renewalPeriod
            };

            // Verify leave type exists
            var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
            if (leaveType == null)
            {
                return NotFound(new { error = $"Leave type with ID {leaveTypeId} not found" });
            }

            // Check if policy already exists
            var existingPolicy = await _context.LeavePolicies
                .FirstOrDefaultAsync(lp => lp.LeaveTypeId == leaveTypeId);

            LeavePolicy policy;

            if (existingPolicy != null)
            {
                // Update existing policy
                existingPolicy.EntitlementAmount = createDto.EntitlementAmount;
                existingPolicy.EntitlementUnit = createDto.EntitlementUnit;
                existingPolicy.RenewalPeriod = createDto.RenewalPeriod;
                existingPolicy.UpdatedAt = DateTime.UtcNow;
                policy = existingPolicy;
            }
            else
            {
                // Create new policy
                policy = new LeavePolicy
                {
                    Id = Guid.NewGuid(),
                    LeaveTypeId = leaveTypeId,
                    EntitlementAmount = createDto.EntitlementAmount,
                    EntitlementUnit = createDto.EntitlementUnit,
                    RenewalPeriod = createDto.RenewalPeriod,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.LeavePolicies.Add(policy);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                policy.Id,
                policy.LeaveTypeId,
                LeaveTypeName = leaveType.Name,
                leaveType.IsSickLeave,
                policy.EntitlementAmount,
                policy.EntitlementUnit,
                RenewalPeriod = policy.RenewalPeriod.ToString(), // Convert enum to string
                policy.CreatedAt,
                policy.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", message = ex.Message });
        }
    }

    /// <summary>
    /// Updates a leave policy.
    /// </summary>
    /// <param name="id">Leave policy ID</param>
    /// <param name="updateDto">Leave policy update data</param>
    /// <returns>Updated leave policy</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateLeavePolicy(Guid id, [FromBody] UpdateLeavePolicyDto updateDto)
    {
        try
        {
            var policy = await _context.LeavePolicies
                .Include(lp => lp.LeaveType)
                .FirstOrDefaultAsync(lp => lp.Id == id);

            if (policy == null)
            {
                return NotFound(new { error = $"Leave policy with ID {id} not found" });
            }

            policy.EntitlementAmount = updateDto.EntitlementAmount;
            policy.EntitlementUnit = updateDto.EntitlementUnit;
            policy.RenewalPeriod = updateDto.RenewalPeriod;
            policy.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                policy.Id,
                policy.LeaveTypeId,
                LeaveTypeName = policy.LeaveType.Name,
                policy.LeaveType.IsSickLeave,
                policy.EntitlementAmount,
                policy.EntitlementUnit,
                RenewalPeriod = policy.RenewalPeriod.ToString(), // Convert enum to string
                policy.CreatedAt,
                policy.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", message = ex.Message });
        }
    }
}

