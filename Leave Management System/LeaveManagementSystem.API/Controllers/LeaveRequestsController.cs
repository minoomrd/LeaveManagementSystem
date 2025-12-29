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
    /// Gets all leave requests for the current user (from query parameter).
    /// This endpoint is for regular users to get their own requests.
    /// </summary>
    /// <param name="userId">User ID (optional, can be passed as query parameter)</param>
    /// <returns>Collection of leave request DTOs</returns>
    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetMyLeaveRequests([FromQuery] Guid? userId)
    {
        try
        {
            if (!userId.HasValue)
            {
                return BadRequest(new { error = "User ID is required", statusCode = 400 });
            }

            var leaveRequests = await _leaveRequestService.GetLeaveRequestsByUserIdAsync(userId.Value);
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving leave requests", message = ex.Message, statusCode = 500 });
        }
    }

    /// <summary>
    /// Gets all leave requests for a user.
    /// This endpoint works for both admins and regular users.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of leave request DTOs</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequestsByUserId(Guid userId)
    {
        try
        {
            var leaveRequests = await _leaveRequestService.GetLeaveRequestsByUserIdAsync(userId);
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving leave requests", message = ex.Message, statusCode = 500 });
        }
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
            return NotFound(new { error = $"Leave request with ID {id} not found", statusCode = 404 });

        return Ok(leaveRequest);
    }

    /// <summary>
    /// Creates a new leave request.
    /// </summary>
    /// <param name="userId">User ID submitting the request</param>
    /// <param name="createLeaveRequestDto">Leave request creation data</param>
    /// <returns>Created leave request DTO</returns>
    [HttpPost("user/{userId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(
        [FromRoute] Guid userId, 
        [FromBody] CreateLeaveRequestDto createLeaveRequestDto)
    {
        try
        {
            var leaveRequest = await _leaveRequestService.CreateLeaveRequestAsync(userId, createLeaveRequestDto);
            // Return 201 Created with the leave request in the response body
            return StatusCode(201, leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message, statusCode = 404 });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message, statusCode = 400 });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message, statusCode = 400 });
        }
        catch (Exception ex)
        {
            // Catch-all for any unexpected exceptions
            return StatusCode(500, new { error = "An unexpected error occurred", message = ex.Message, statusCode = 500 });
        }
    }

    /// <summary>
    /// Reviews a leave request (approve or reject).
    /// </summary>
    /// <param name="id">Leave request ID</param>
    /// <param name="reviewDto">Review data with status ("accept" or "reject") and optional admin comment</param>
    /// <returns>Updated leave request DTO</returns>
    [HttpPost("{id}/review")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<LeaveRequestDto>> ReviewLeaveRequest(
        Guid id, 
        [FromBody] ReviewLeaveRequestDto reviewDto)
    {
        try
        {
            // Validate and convert status string to enum
            Domain.Enums.LeaveRequestStatus status;
            var statusLower = reviewDto.Status?.Trim().ToLowerInvariant();
            
            if (string.IsNullOrWhiteSpace(statusLower))
            {
                return BadRequest(new { error = "Status is required. Must be 'accept' or 'reject'", statusCode = 400 });
            }

            if (statusLower == "accept" || statusLower == "approve")
            {
                status = Domain.Enums.LeaveRequestStatus.Approved;
            }
            else if (statusLower == "reject")
            {
                status = Domain.Enums.LeaveRequestStatus.Rejected;
            }
            else
            {
                return BadRequest(new { error = "Status must be 'accept' or 'reject'", statusCode = 400 });
            }

            // Create UpdateLeaveRequestStatusDto from ReviewLeaveRequestDto
            var updateStatusDto = new UpdateLeaveRequestStatusDto
            {
                Status = status,
                AdminComment = reviewDto.AdminComment
            };

            var leaveRequest = await _leaveRequestService.UpdateLeaveRequestStatusAsync(id, updateStatusDto);
            return Ok(leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message, statusCode = 404 });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message, statusCode = 400 });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message, statusCode = 400 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An unexpected error occurred", message = ex.Message, statusCode = 500 });
        }
    }

}

