using LeaveManagementSystem.Application.DTOs;

namespace LeaveManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for role operations.
/// Following Interface Segregation Principle - defines only role-related operations.
/// Following Dependency Inversion Principle - high-level modules depend on abstraction.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="createRoleDto">Role creation data</param>
    /// <returns>Created role DTO</returns>
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);

    /// <summary>
    /// Gets a role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role DTO if found, null otherwise</returns>
    Task<RoleDto?> GetRoleByIdAsync(Guid id);

    /// <summary>
    /// Gets a role by name
    /// </summary>
    /// <param name="name">Role name</param>
    /// <returns>Role DTO if found, null otherwise</returns>
    Task<RoleDto?> GetRoleByNameAsync(string name);

    /// <summary>
    /// Gets all roles
    /// </summary>
    /// <returns>Collection of role DTOs</returns>
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();

    /// <summary>
    /// Updates a role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="updateRoleDto">Role update data</param>
    /// <returns>Updated role DTO</returns>
    Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto);

    /// <summary>
    /// Deletes a role
    /// </summary>
    /// <param name="id">Role ID</param>
    Task DeleteRoleAsync(Guid id);
}

