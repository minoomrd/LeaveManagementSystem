using LeaveManagementSystem.Application.DTOs;

namespace LeaveManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for leave request operations.
/// Following Interface Segregation Principle - defines only leave request-related operations.
/// Following Dependency Inversion Principle - high-level modules depend on abstraction.
/// </summary>
public interface ILeaveRequestService
{
    /// <summary>
    /// Creates a new leave request
    /// </summary>
    /// <param name="userId">User ID submitting the request</param>
    /// <param name="createLeaveRequestDto">Leave request creation data</param>
    /// <returns>Created leave request DTO</returns>
    Task<LeaveRequestDto> CreateLeaveRequestAsync(Guid userId, CreateLeaveRequestDto createLeaveRequestDto);

    /// <summary>
    /// Gets a leave request by ID
    /// </summary>
    /// <param name="id">Leave request ID</param>
    /// <returns>Leave request DTO if found, null otherwise</returns>
    Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id);

    /// <summary>
    /// Gets all leave requests for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of leave request DTOs</returns>
    Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdAsync(Guid userId);

    /// <summary>
    /// Updates the status of a leave request (Approved or Rejected)
    /// </summary>
    /// <param name="leaveRequestId">Leave request ID</param>
    /// <param name="updateStatusDto">Status update data</param>
    /// <returns>Updated leave request DTO</returns>
    Task<LeaveRequestDto> UpdateLeaveRequestStatusAsync(Guid leaveRequestId, UpdateLeaveRequestStatusDto updateStatusDto);
}

