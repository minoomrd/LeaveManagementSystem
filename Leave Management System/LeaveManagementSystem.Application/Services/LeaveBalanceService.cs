using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.Services;

/// <summary>
/// Service implementation for leave balance operations.
/// Following Single Responsibility Principle - handles only leave balance-related business logic.
/// Following Open/Closed Principle - can be extended without modification through interfaces.
/// </summary>
public class LeaveBalanceService : ILeaveBalanceService
{
    private readonly ILeaveBalanceRepository _leaveBalanceRepository;
    private readonly IRepository<LeaveType> _leaveTypeRepository;
    private readonly IRepository<LeavePolicy> _leavePolicyRepository;
    private readonly IRepository<EmployeeLeaveSetting> _employeeLeaveSettingRepository;

    /// <summary>
    /// Initializes a new instance of the LeaveBalanceService class.
    /// Following Dependency Inversion Principle - depends on abstractions.
    /// </summary>
    public LeaveBalanceService(
        ILeaveBalanceRepository leaveBalanceRepository,
        IRepository<LeaveType> leaveTypeRepository,
        IRepository<LeavePolicy> leavePolicyRepository,
        IRepository<EmployeeLeaveSetting> employeeLeaveSettingRepository)
    {
        _leaveBalanceRepository = leaveBalanceRepository ?? throw new ArgumentNullException(nameof(leaveBalanceRepository));
        _leaveTypeRepository = leaveTypeRepository ?? throw new ArgumentNullException(nameof(leaveTypeRepository));
        _leavePolicyRepository = leavePolicyRepository ?? throw new ArgumentNullException(nameof(leavePolicyRepository));
        _employeeLeaveSettingRepository = employeeLeaveSettingRepository ?? throw new ArgumentNullException(nameof(employeeLeaveSettingRepository));
    }

    /// <summary>
    /// Gets leave balances for a user.
    /// Uses specialized repository method with JOINs to avoid DbContext concurrency issues.
    /// </summary>
    public async Task<IEnumerable<LeaveBalanceDto>> GetLeaveBalancesByUserIdAsync(Guid userId)
    {
        // Use specialized repository method that does JOINs in a single query
        // This completely avoids DbContext concurrency issues
        return await _leaveBalanceRepository.GetLeaveBalancesByUserIdWithDetailsAsync(userId);
    }

    /// <summary>
    /// Gets a specific leave balance for a user and leave type.
    /// </summary>
    public async Task<LeaveBalanceDto?> GetLeaveBalanceAsync(Guid userId, Guid leaveTypeId)
    {
        var leaveBalances = await _leaveBalanceRepository.FindAsync(lb =>
            lb.UserId == userId && lb.LeaveTypeId == leaveTypeId);

        var leaveBalance = leaveBalances.FirstOrDefault();
        return leaveBalance != null ? await MapToDtoAsync(leaveBalance) : null;
    }

    /// <summary>
    /// Gets or creates a leave balance for a user and leave type.
    /// If the balance doesn't exist, it will be created with the default entitlement.
    /// </summary>
    public async Task<LeaveBalanceDto> GetOrCreateLeaveBalanceAsync(Guid userId, Guid leaveTypeId, LeaveUnit defaultUnit)
    {
        var leaveBalances = await _leaveBalanceRepository.FindAsync(lb =>
            lb.UserId == userId && lb.LeaveTypeId == leaveTypeId);

        var leaveBalance = leaveBalances.FirstOrDefault();

        if (leaveBalance == null)
        {
            // Create new balance if it doesn't exist
            // First, get the entitlement amount
            decimal entitlement;
            try
            {
                entitlement = await GetEntitlementAmountAsync(userId, leaveTypeId);
            }
            catch (InvalidOperationException)
            {
                // If no policy exists, use a default value based on unit
                // This ensures the system can still function even without policies
                entitlement = defaultUnit == LeaveUnit.Day ? 20 : 40; // Default: 20 days or 40 hours
            }

            leaveBalance = new LeaveBalance
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LeaveTypeId = leaveTypeId,
                BalanceAmount = entitlement,
                BalanceUnit = defaultUnit,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            leaveBalance = await _leaveBalanceRepository.AddAsync(leaveBalance);
        }

        return await MapToDtoAsync(leaveBalance);
    }

    /// <summary>
    /// Updates leave balance after a leave request is approved.
    /// Deducts the duration from the current balance.
    /// </summary>
    public async Task UpdateLeaveBalanceAsync(Guid userId, Guid leaveTypeId, decimal durationAmount, LeaveUnit durationUnit)
    {
        // Get or create leave balance
        var leaveBalances = await _leaveBalanceRepository.FindAsync(lb =>
            lb.UserId == userId && lb.LeaveTypeId == leaveTypeId);

        var leaveBalance = leaveBalances.FirstOrDefault();

        if (leaveBalance == null)
        {
            // Create new balance if it doesn't exist
            // First, get the entitlement amount
            var entitlement = await GetEntitlementAmountAsync(userId, leaveTypeId);

            leaveBalance = new LeaveBalance
            {
                UserId = userId,
                LeaveTypeId = leaveTypeId,
                BalanceAmount = entitlement,
                BalanceUnit = durationUnit
            };

            leaveBalance = await _leaveBalanceRepository.AddAsync(leaveBalance);
        }

        // Deduct the duration from balance
        // Note: In a real system, you might want to convert units if they don't match
        if (leaveBalance.BalanceUnit == durationUnit)
        {
            leaveBalance.BalanceAmount -= durationAmount;
        }
        else
        {
            // Convert if units don't match (e.g., hours to days)
            // This is a simplified conversion - adjust based on business rules
            if (durationUnit == LeaveUnit.Hour && leaveBalance.BalanceUnit == LeaveUnit.Day)
            {
                // Assuming 8 hours per day
                leaveBalance.BalanceAmount -= durationAmount / 8;
            }
            else if (durationUnit == LeaveUnit.Day && leaveBalance.BalanceUnit == LeaveUnit.Hour)
            {
                // Assuming 8 hours per day
                leaveBalance.BalanceAmount -= durationAmount * 8;
            }
        }

        leaveBalance.UpdatedAt = DateTime.UtcNow;
        await _leaveBalanceRepository.UpdateAsync(leaveBalance);
    }

    /// <summary>
    /// Adds back to leave balance when a leave request is rejected after being approved.
    /// Adds the duration back to the current balance.
    /// </summary>
    public async Task AddToLeaveBalanceAsync(Guid userId, Guid leaveTypeId, decimal durationAmount, LeaveUnit durationUnit)
    {
        // Get or create leave balance
        var leaveBalances = await _leaveBalanceRepository.FindAsync(lb =>
            lb.UserId == userId && lb.LeaveTypeId == leaveTypeId);

        var leaveBalance = leaveBalances.FirstOrDefault();

        if (leaveBalance == null)
        {
            // Create new balance if it doesn't exist
            // First, get the entitlement amount
            var entitlement = await GetEntitlementAmountAsync(userId, leaveTypeId);

            leaveBalance = new LeaveBalance
            {
                UserId = userId,
                LeaveTypeId = leaveTypeId,
                BalanceAmount = entitlement,
                BalanceUnit = durationUnit
            };

            leaveBalance = await _leaveBalanceRepository.AddAsync(leaveBalance);
        }

        // Add the duration back to balance
        // Note: In a real system, you might want to convert units if they don't match
        if (leaveBalance.BalanceUnit == durationUnit)
        {
            leaveBalance.BalanceAmount += durationAmount;
        }
        else
        {
            // Convert if units don't match (e.g., hours to days)
            // This is a simplified conversion - adjust based on business rules
            if (durationUnit == LeaveUnit.Hour && leaveBalance.BalanceUnit == LeaveUnit.Day)
            {
                // Assuming 8 hours per day
                leaveBalance.BalanceAmount += durationAmount / 8;
            }
            else if (durationUnit == LeaveUnit.Day && leaveBalance.BalanceUnit == LeaveUnit.Hour)
            {
                // Assuming 8 hours per day
                leaveBalance.BalanceAmount += durationAmount * 8;
            }
        }

        leaveBalance.UpdatedAt = DateTime.UtcNow;
        await _leaveBalanceRepository.UpdateAsync(leaveBalance);
    }

    /// <summary>
    /// Gets the entitlement amount for a user and leave type.
    /// Checks for custom employee settings first, then falls back to default policy.
    /// </summary>
    private async Task<decimal> GetEntitlementAmountAsync(Guid userId, Guid leaveTypeId)
    {
        // Check for custom employee setting
        var customSettings = await _employeeLeaveSettingRepository.FindAsync(els =>
            els.UserId == userId && els.LeaveTypeId == leaveTypeId);

        var customSetting = customSettings.FirstOrDefault();
        if (customSetting != null)
        {
            return customSetting.CustomEntitlementAmount;
        }

        // Fall back to default policy
        var policies = await _leavePolicyRepository.FindAsync(lp => lp.LeaveTypeId == leaveTypeId);
        var policy = policies.FirstOrDefault();

        if (policy == null)
            throw new InvalidOperationException($"No leave policy found for leave type {leaveTypeId}");

        return policy.EntitlementAmount;
    }

    /// <summary>
    /// Maps LeaveBalance entity to LeaveBalanceDto.
    /// </summary>
    private async Task<LeaveBalanceDto> MapToDtoAsync(LeaveBalance leaveBalance)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveBalance.LeaveTypeId);

        return new LeaveBalanceDto
        {
            Id = leaveBalance.Id,
            UserId = leaveBalance.UserId,
            LeaveTypeId = leaveBalance.LeaveTypeId,
            LeaveTypeName = leaveType?.Name ?? string.Empty,
            BalanceAmount = leaveBalance.BalanceAmount,
            BalanceUnit = leaveBalance.BalanceUnit,
            UpdatedAt = leaveBalance.UpdatedAt
        };
    }
}

