using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Data;

/// <summary>
/// Seed data for initial database population.
/// Following Single Responsibility Principle - handles only data seeding.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Seeds the database with initial data.
    /// </summary>
    /// <param name="context">Database context</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Always ensure leave types exist (they might be missing even if users exist)
        await SeedLeaveTypesAsync(context);

        // Check if other data already exists
        if (await context.Users.AnyAsync())
        {
            return; // Other data already seeded
        }

        // Create Roles first (using fixed GUIDs to match migration)
        var employeeRoleId = new Guid("11111111-1111-1111-1111-111111111111");
        var adminRoleId = new Guid("22222222-2222-2222-2222-222222222222");
        var superAdminRoleId = new Guid("33333333-3333-3333-3333-333333333333");

        // Check if roles already exist (from migration)
        var existingEmployeeRole = await context.Roles.FindAsync(employeeRoleId);
        var existingAdminRole = await context.Roles.FindAsync(adminRoleId);
        var existingSuperAdminRole = await context.Roles.FindAsync(superAdminRoleId);

        var employeeRole = existingEmployeeRole ?? new Role
        {
            Id = employeeRoleId,
            Name = "Employee",
            Description = "Regular employee who can request leaves",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var adminRole = existingAdminRole ?? new Role
        {
            Id = adminRoleId,
            Name = "Admin",
            Description = "Company administrator who manages employees and approves leaves",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var superAdminRole = existingSuperAdminRole ?? new Role
        {
            Id = superAdminRoleId,
            Name = "SuperAdmin",
            Description = "Platform super-admin who manages companies and billing",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (existingEmployeeRole == null) context.Roles.Add(employeeRole);
        if (existingAdminRole == null) context.Roles.Add(adminRole);
        if (existingSuperAdminRole == null) context.Roles.Add(superAdminRole);

        // Get leave types (already created by SeedLeaveTypesAsync)
        var dailyLeaveType = await context.LeaveTypes.FirstAsync(lt => lt.Name == "Daily");
        var hourlyLeaveType = await context.LeaveTypes.FirstAsync(lt => lt.Name == "Hourly");

        // Create Company
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Acme Corporation",
            ContactEmail = "admin@acme.com",
            Status = CompanyStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Companies.Add(company);

        // Create Admin User (password: Admin123!)
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Admin User",
            Email = "admin@acme.com",
            PasswordHash = HashPassword("Admin123!"), // In production, use proper hashing
            RoleId = adminRole.Id,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        company.OwnerId = adminUser.Id;

        // Create Employee Users (password: Employee123!)
        var employee1 = new User
        {
            Id = Guid.NewGuid(),
            FullName = "John Doe",
            Email = "john.doe@acme.com",
            PasswordHash = HashPassword("Employee123!"),
            RoleId = employeeRole.Id,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var employee2 = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Jane Smith",
            Email = "jane.smith@acme.com",
            PasswordHash = HashPassword("Employee123!"),
            RoleId = employeeRole.Id,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, employee1, employee2);

        // Create Leave Balances (only if they don't exist)
        var employee1DailyBalance = new LeaveBalance
        {
            Id = Guid.NewGuid(),
            UserId = employee1.Id,
            LeaveTypeId = dailyLeaveType.Id,
            BalanceAmount = 15,
            BalanceUnit = LeaveUnit.Day,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var employee1HourlyBalance = new LeaveBalance
        {
            Id = Guid.NewGuid(),
            UserId = employee1.Id,
            LeaveTypeId = hourlyLeaveType.Id,
            BalanceAmount = 35,
            BalanceUnit = LeaveUnit.Hour,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var employee2DailyBalance = new LeaveBalance
        {
            Id = Guid.NewGuid(),
            UserId = employee2.Id,
            LeaveTypeId = dailyLeaveType.Id,
            BalanceAmount = 18,
            BalanceUnit = LeaveUnit.Day,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var employee2HourlyBalance = new LeaveBalance
        {
            Id = Guid.NewGuid(),
            UserId = employee2.Id,
            LeaveTypeId = hourlyLeaveType.Id,
            BalanceAmount = 30,
            BalanceUnit = LeaveUnit.Hour,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.LeaveBalances.AddRange(
            employee1DailyBalance,
            employee1HourlyBalance,
            employee2DailyBalance,
            employee2HourlyBalance
        );

        // Create Sample Leave Requests
        var leaveRequest1 = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            UserId = employee1.Id,
            LeaveTypeId = dailyLeaveType.Id,
            StartDateTime = DateTime.UtcNow.AddDays(5),
            EndDateTime = DateTime.UtcNow.AddDays(7),
            DurationAmount = 3,
            DurationUnit = LeaveUnit.Day,
            Reason = "Family vacation",
            Status = LeaveRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var leaveRequest2 = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            UserId = employee2.Id,
            LeaveTypeId = hourlyLeaveType.Id,
            StartDateTime = DateTime.UtcNow.AddDays(-5),
            EndDateTime = DateTime.UtcNow.AddDays(-5),
            DurationAmount = 4,
            DurationUnit = LeaveUnit.Hour,
            Reason = "Medical appointment",
            Status = LeaveRequestStatus.Approved,
            AdminComment = "Approved",
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        context.LeaveRequests.AddRange(leaveRequest1, leaveRequest2);

        // Create Company User Limit
        var companyUserLimit = new CompanyUserLimit
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            FreeUserLimit = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.CompanyUserLimits.Add(companyUserLimit);

        // Save all changes
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds leave types (always ensures they exist).
    /// </summary>
    public static async Task SeedLeaveTypesAsync(ApplicationDbContext context)
    {
        try
        {
            // Ensure database is created and migrations are applied
            await context.Database.EnsureCreatedAsync();
            
            var dailyLeaveType = await GetOrCreateLeaveTypeAsync(context, "Daily", LeaveUnit.Day, "Daily leave measured in days");
            var hourlyLeaveType = await GetOrCreateLeaveTypeAsync(context, "Hourly", LeaveUnit.Hour, "Hourly leave measured in hours");
            
            // Create policies if they don't exist
            try
            {
                var existingDailyPolicy = await context.LeavePolicies.FirstOrDefaultAsync(lp => lp.LeaveTypeId == dailyLeaveType.Id);
                var existingHourlyPolicy = await context.LeavePolicies.FirstOrDefaultAsync(lp => lp.LeaveTypeId == hourlyLeaveType.Id);

                if (existingDailyPolicy == null)
                {
                    context.LeavePolicies.Add(new LeavePolicy
                    {
                        Id = Guid.NewGuid(),
                        LeaveTypeId = dailyLeaveType.Id,
                        EntitlementAmount = 20,
                        EntitlementUnit = LeaveUnit.Day,
                        RenewalPeriod = RenewalPeriod.Yearly,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                if (existingHourlyPolicy == null)
                {
                    context.LeavePolicies.Add(new LeavePolicy
                    {
                        Id = Guid.NewGuid(),
                        LeaveTypeId = hourlyLeaveType.Id,
                        EntitlementAmount = 40,
                        EntitlementUnit = LeaveUnit.Hour,
                        RenewalPeriod = RenewalPeriod.Monthly,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                await context.SaveChangesAsync();
            }
            catch (Exception policyEx)
            {
                // Policies might fail if table doesn't exist, but leave types should still be created
                Console.WriteLine($"Warning: Could not create leave policies: {policyEx.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SeedLeaveTypesAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Gets an existing leave type or creates a new one.
    /// </summary>
    private static async Task<LeaveType> GetOrCreateLeaveTypeAsync(ApplicationDbContext context, string name, LeaveUnit unit, string description)
    {
        try
        {
            var existingLeaveType = await context.LeaveTypes.FirstOrDefaultAsync(lt => lt.Name == name);
            
            if (existingLeaveType != null)
            {
                return existingLeaveType;
            }

            var newLeaveType = new LeaveType
            {
                Id = Guid.NewGuid(),
                Name = name,
                Unit = unit,
                Description = description,
                IsSickLeave = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.LeaveTypes.Add(newLeaveType);
            await context.SaveChangesAsync();
            
            return newLeaveType;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetOrCreateLeaveTypeAsync for {name}: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Simple password hashing (SHA256).
    /// In production, use bcrypt or Argon2.
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}




