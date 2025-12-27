using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Data;
using LeaveManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagementSystem.Infrastructure;

/// <summary>
/// Extension class for dependency injection configuration.
/// Following Single Responsibility Principle - handles only dependency injection setup.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// Following Dependency Inversion Principle - registers implementations with interfaces.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add PostgreSQL database context
        // Configure to not fail on startup if database is unavailable
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    // Set command timeout to fail faster (1 second)
                    npgsqlOptions.CommandTimeout(1);
                    // DO NOT enable retry on failure - we want to fail fast when PostgreSQL is not available
                    // By not calling EnableRetryOnFailure(), we avoid retries
                });
            
            // Don't validate connection on startup - allow lazy loading
            // This allows API to start even if database is unavailable
            options.EnableSensitiveDataLogging();
        });

        // Register repositories
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<Company>, Repository<Company>>();
        services.AddScoped<IRepository<LeaveType>, Repository<LeaveType>>();
        services.AddScoped<IRepository<LeavePolicy>, Repository<LeavePolicy>>();
        services.AddScoped<IRepository<EmployeeLeaveSetting>, Repository<EmployeeLeaveSetting>>();
        // Register specialized LeaveRequestRepository (implements both IRepository<LeaveRequest> and ILeaveRequestRepository)
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddScoped<IRepository<LeaveRequest>>(sp => sp.GetRequiredService<ILeaveRequestRepository>());
        services.AddScoped<IRepository<LeaveBalance>, Repository<LeaveBalance>>();
        services.AddScoped<IRepository<WorkingHours>, Repository<WorkingHours>>();
        services.AddScoped<IRepository<CompanyUserLimit>, Repository<CompanyUserLimit>>();
        services.AddScoped<IRepository<PricingTier>, Repository<PricingTier>>();
        services.AddScoped<IRepository<CompanySubscription>, Repository<CompanySubscription>>();
        services.AddScoped<IRepository<Payment>, Repository<Payment>>();
        services.AddScoped<IRepository<Domain.Entities.Role>, Repository<Domain.Entities.Role>>();

        return services;
    }
}

