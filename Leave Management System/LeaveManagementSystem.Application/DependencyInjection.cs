using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagementSystem.Application;

/// <summary>
/// Extension class for dependency injection configuration.
/// Following Single Responsibility Principle - handles only application service registration.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the service collection.
    /// Following Dependency Inversion Principle - registers implementations with interfaces.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}

