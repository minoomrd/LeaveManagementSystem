using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

/// <summary>
/// Controller for leave request operations.
/// Following Single Responsibility Principle - handles only leave request-related HTTP requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    /// <summary>
    /// Initializes a new instance of the LeaveRequestsController class.
    /// Following Dependency Inversion Principle - depends on ILeaveRequestService abstraction.
    /// </summary>
    /// <param name="leaveRequestService">Leave request service</param>
    public LeaveRequestsController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService ?? throw new ArgumentNullException(nameof(leaveRequestService));
    }

    /// <summary>
    /// Gets all leave requests for a user.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of leave request DTOs</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequestsByUserId(Guid userId)
    {
        var leaveRequests = await _leaveRequestService.GetLeaveRequestsByUserIdAsync(userId);
        return Ok(leaveRequests);
    }

    /// <summary>
    /// Gets a leave request by ID.
    /// </summary>
    /// <param name="id">Leave request ID</param>
    /// <returns>Leave request DTO</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequestById(Guid id)
    {
        var leaveRequest = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
        if (leaveRequest == null)
            return NotFound($"Leave request with ID {id} not found");

        return Ok(leaveRequest);
    }

    /// <summary>
    /// Creates a new leave request.
    /// </summary>
    /// <param name="userId">User ID submitting the request</param>
    /// <param name="createLeaveRequestDto">Leave request creation data</param>
    /// <returns>Created leave request DTO</returns>
    [HttpPost("user/{userId}")]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(Guid userId, [FromBody] CreateLeaveRequestDto createLeaveRequestDto)
    {
        try
        {
            var leaveRequest = await _leaveRequestService.CreateLeaveRequestAsync(userId, createLeaveRequestDto);
            return CreatedAtAction(nameof(GetLeaveRequestById), new { id = leaveRequest.Id }, leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Approves a leave request.
    /// </summary>
    /// <param name="id">Leave request ID</param>
    /// <param name="approveDto">Approval data</param>
    /// <returns>Updated leave request DTO</returns>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<LeaveRequestDto>> ApproveLeaveRequest(Guid id, [FromBody] ApproveLeaveRequestDto approveDto)
    {
        try
        {
            var leaveRequest = await _leaveRequestService.ApproveLeaveRequestAsync(id, approveDto);
            return Ok(leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Rejects a leave request.
    /// </summary>
    /// <param name="id">Leave request ID</param>
    /// <param name="approveDto">Rejection data</param>
    /// <returns>Updated leave request DTO</returns>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<LeaveRequestDto>> RejectLeaveRequest(Guid id, [FromBody] ApproveLeaveRequestDto approveDto)
    {
        try
        {
            var leaveRequest = await _leaveRequestService.RejectLeaveRequestAsync(id, approveDto);
            return Ok(leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

