using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.Services;

/// <summary>
/// Service implementation for leave request operations.
/// Following Single Responsibility Principle - handles only leave request-related business logic.
/// Following Open/Closed Principle - can be extended without modification through interfaces.
/// </summary>
public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<LeaveType> _leaveTypeRepository;
    private readonly ILeaveBalanceService _leaveBalanceService;

    /// <summary>
    /// Initializes a new instance of the LeaveRequestService class.
    /// Following Dependency Inversion Principle - depends on abstractions.
    /// </summary>
    public LeaveRequestService(
        ILeaveRequestRepository leaveRequestRepository,
        IRepository<User> userRepository,
        IRepository<LeaveType> leaveTypeRepository,
        ILeaveBalanceService leaveBalanceService)
    {
        _leaveRequestRepository = leaveRequestRepository ?? throw new ArgumentNullException(nameof(leaveRequestRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _leaveTypeRepository = leaveTypeRepository ?? throw new ArgumentNullException(nameof(leaveTypeRepository));
        _leaveBalanceService = leaveBalanceService ?? throw new ArgumentNullException(nameof(leaveBalanceService));
    }

    /// <summary>
    /// Creates a new leave request with validation and duration calculation.
    /// </summary>
    public async Task<LeaveRequestDto> CreateLeaveRequestAsync(Guid userId, CreateLeaveRequestDto createLeaveRequestDto)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        // Find leave type by unit (1 = Hour, 2 = Day)
        var leaveTypes = await _leaveTypeRepository.FindAsync(lt => lt.Unit == createLeaveRequestDto.LeaveTypeUnit);
        var leaveType = leaveTypes.FirstOrDefault();
        if (leaveType == null)
        {
            // If leave type doesn't exist, create it
            leaveType = new LeaveType
            {
                Id = Guid.NewGuid(),
                Name = createLeaveRequestDto.LeaveTypeUnit == LeaveUnit.Day ? "Daily" : "Hourly",
                Unit = createLeaveRequestDto.LeaveTypeUnit,
                Description = createLeaveRequestDto.LeaveTypeUnit == LeaveUnit.Day ? "Daily leave measured in days" : "Hourly leave measured in hours",
                IsSickLeave = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            leaveType = await _leaveTypeRepository.AddAsync(leaveType);
        }

        // Validate dates
        if (createLeaveRequestDto.StartDateTime >= createLeaveRequestDto.EndDateTime)
            throw new ArgumentException("Start date must be before end date");

        // Check for overlapping leave requests
        var overlappingRequests = await _leaveRequestRepository.FindAsync(lr =>
            lr.UserId == userId &&
            lr.Status == LeaveRequestStatus.Approved &&
            lr.StartDateTime <= createLeaveRequestDto.EndDateTime &&
            lr.EndDateTime >= createLeaveRequestDto.StartDateTime);

        if (overlappingRequests.Any())
            throw new InvalidOperationException("Leave request overlaps with an existing approved leave");

        // Calculate duration
        var duration = CalculateDuration(
            createLeaveRequestDto.StartDateTime,
            createLeaveRequestDto.EndDateTime,
            leaveType.Unit);

        // Get or create leave balance (creates with default entitlement if doesn't exist)
        var balance = await _leaveBalanceService.GetOrCreateLeaveBalanceAsync(userId, leaveType.Id, leaveType.Unit);
        
        // Check if balance is sufficient
        if (balance.BalanceAmount < duration)
            throw new InvalidOperationException($"Insufficient leave balance. Available: {balance.BalanceAmount} {balance.BalanceUnit}, Required: {duration} {leaveType.Unit}");

        // Create leave request
        var leaveRequest = new LeaveRequest
        {
            UserId = userId,
            LeaveTypeId = leaveType.Id,
            StartDateTime = createLeaveRequestDto.StartDateTime,
            EndDateTime = createLeaveRequestDto.EndDateTime,
            DurationAmount = duration,
            DurationUnit = leaveType.Unit,
            Reason = createLeaveRequestDto.Reason,
            Status = LeaveRequestStatus.Pending
        };

        var createdRequest = await _leaveRequestRepository.AddAsync(leaveRequest);

        return await MapToDtoAsync(createdRequest);
    }

    /// <summary>
    /// Gets a leave request by ID.
    /// </summary>
    public async Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
        return leaveRequest != null ? await MapToDtoAsync(leaveRequest) : null;
    }

    /// <summary>
    /// Gets all leave requests for a user.
    /// Uses specialized repository method with JOINs to avoid DbContext concurrency issues.
    /// </summary>
    public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdAsync(Guid userId)
    {
        // Use specialized repository method that does JOINs in a single query
        // This completely avoids DbContext concurrency issues
        return await _leaveRequestRepository.GetLeaveRequestsByUserIdWithDetailsAsync(userId);
    }

    /// <summary>
    /// Approves a leave request and updates leave balance.
    /// </summary>
    public async Task<LeaveRequestDto> ApproveLeaveRequestAsync(Guid leaveRequestId, ApproveLeaveRequestDto approveDto)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(leaveRequestId);
        if (leaveRequest == null)
            throw new KeyNotFoundException($"Leave request with ID {leaveRequestId} not found");

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be approved");

        // Update status
        leaveRequest.Status = LeaveRequestStatus.Approved;
        leaveRequest.AdminComment = approveDto.AdminComment;
        leaveRequest.UpdatedAt = DateTime.UtcNow;

        await _leaveRequestRepository.UpdateAsync(leaveRequest);

        // Update leave balance
        await _leaveBalanceService.UpdateLeaveBalanceAsync(
            leaveRequest.UserId,
            leaveRequest.LeaveTypeId,
            leaveRequest.DurationAmount,
            leaveRequest.DurationUnit);

        return await MapToDtoAsync(leaveRequest);
    }

    /// <summary>
    /// Rejects a leave request.
    /// </summary>
    public async Task<LeaveRequestDto> RejectLeaveRequestAsync(Guid leaveRequestId, ApproveLeaveRequestDto approveDto)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(leaveRequestId);
        if (leaveRequest == null)
            throw new KeyNotFoundException($"Leave request with ID {leaveRequestId} not found");

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be rejected");

        // Update status
        leaveRequest.Status = LeaveRequestStatus.Rejected;
        leaveRequest.AdminComment = approveDto.AdminComment;
        leaveRequest.UpdatedAt = DateTime.UtcNow;

        await _leaveRequestRepository.UpdateAsync(leaveRequest);

        return await MapToDtoAsync(leaveRequest);
    }

    /// <summary>
    /// Calculates the duration of leave based on start and end dates.
    /// </summary>
    private static decimal CalculateDuration(DateTime startDateTime, DateTime endDateTime, LeaveUnit unit)
    {
        if (unit == LeaveUnit.Day)
        {
            // Calculate days (inclusive of both start and end dates)
            var days = (endDateTime.Date - startDateTime.Date).Days + 1;
            return days;
        }
        else // Hour
        {
            // Calculate hours
            var hours = (endDateTime - startDateTime).TotalHours;
            return (decimal)hours;
        }
    }

    /// <summary>
    /// Maps LeaveRequest entity to LeaveRequestDto.
    /// </summary>
    private async Task<LeaveRequestDto> MapToDtoAsync(LeaveRequest leaveRequest)
    {
        var user = await _userRepository.GetByIdAsync(leaveRequest.UserId);
        var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveRequest.LeaveTypeId);

        return new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            UserId = leaveRequest.UserId,
            UserName = user?.FullName ?? string.Empty,
            LeaveTypeId = leaveRequest.LeaveTypeId,
            LeaveTypeName = leaveType?.Name ?? string.Empty,
            StartDateTime = leaveRequest.StartDateTime,
            EndDateTime = leaveRequest.EndDateTime,
            DurationAmount = leaveRequest.DurationAmount,
            DurationUnit = leaveRequest.DurationUnit,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            AdminComment = leaveRequest.AdminComment,
            CreatedAt = leaveRequest.CreatedAt
        };
    }
}

