using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

/// <summary>
/// Controller for leave balance operations.
/// Following Single Responsibility Principle - handles only leave balance-related HTTP requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeaveBalancesController : ControllerBase
{
    private readonly ILeaveBalanceService _leaveBalanceService;

    /// <summary>
    /// Initializes a new instance of the LeaveBalancesController class.
    /// Following Dependency Inversion Principle - depends on ILeaveBalanceService abstraction.
    /// </summary>
    /// <param name="leaveBalanceService">Leave balance service</param>
    public LeaveBalancesController(ILeaveBalanceService leaveBalanceService)
    {
        _leaveBalanceService = leaveBalanceService ?? throw new ArgumentNullException(nameof(leaveBalanceService));
    }

    /// <summary>
    /// Gets all leave balances for a user.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of leave balance DTOs</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<LeaveBalanceDto>>> GetLeaveBalancesByUserId(Guid userId)
    {
        var leaveBalances = await _leaveBalanceService.GetLeaveBalancesByUserIdAsync(userId);
        return Ok(leaveBalances);
    }

    /// <summary>
    /// Gets a specific leave balance for a user and leave type.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="leaveTypeId">Leave type ID</param>
    /// <returns>Leave balance DTO</returns>
    [HttpGet("user/{userId}/leave-type/{leaveTypeId}")]
    public async Task<ActionResult<LeaveBalanceDto>> GetLeaveBalance(Guid userId, Guid leaveTypeId)
    {
        var leaveBalance = await _leaveBalanceService.GetLeaveBalanceAsync(userId, leaveTypeId);
        if (leaveBalance == null)
            return NotFound($"Leave balance not found for user {userId} and leave type {leaveTypeId}");

        return Ok(leaveBalance);
    }
}

