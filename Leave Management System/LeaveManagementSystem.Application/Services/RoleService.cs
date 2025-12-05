using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Application.Services;

/// <summary>
/// Service implementation for role operations.
/// Following Single Responsibility Principle - handles only role-related business logic.
/// Following Open/Closed Principle - can be extended without modification through interfaces.
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepository;

    /// <summary>
    /// Initializes a new instance of the RoleService class.
    /// Following Dependency Inversion Principle - depends on abstraction (IRepository).
    /// </summary>
    /// <param name="roleRepository">Repository for role data access</param>
    public RoleService(IRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(createRoleDto.Name))
            throw new ArgumentException("Role name is required", nameof(createRoleDto));

        // Check if role already exists
        var existingRole = await _roleRepository.FindAsync(r => r.Name == createRoleDto.Name);
        if (existingRole.Any())
            throw new InvalidOperationException("Role with this name already exists");

        // Create new role entity
        var role = new Role
        {
            Name = createRoleDto.Name,
            Description = createRoleDto.Description,
            IsActive = createRoleDto.IsActive
        };

        // Save to database
        var createdRole = await _roleRepository.AddAsync(role);

        // Map to DTO
        return MapToDto(createdRole);
    }

    /// <summary>
    /// Gets a role by ID.
    /// </summary>
    public async Task<RoleDto?> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return role != null ? MapToDto(role) : null;
    }

    /// <summary>
    /// Gets a role by name.
    /// </summary>
    public async Task<RoleDto?> GetRoleByNameAsync(string name)
    {
        var roles = await _roleRepository.FindAsync(r => r.Name == name);
        var role = roles.FirstOrDefault();
        return role != null ? MapToDto(role) : null;
    }

    /// <summary>
    /// Gets all roles.
    /// </summary>
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(MapToDto);
    }

    /// <summary>
    /// Updates a role.
    /// </summary>
    public async Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(updateRoleDto.Name))
            throw new ArgumentException("Role name is required", nameof(updateRoleDto));

        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new KeyNotFoundException($"Role with ID {id} not found");

        // Check if another role with the same name exists
        var existingRole = await _roleRepository.FindAsync(r => r.Name == updateRoleDto.Name && r.Id != id);
        if (existingRole.Any())
            throw new InvalidOperationException("Role with this name already exists");

        // Update properties
        role.Name = updateRoleDto.Name;
        role.Description = updateRoleDto.Description;
        role.IsActive = updateRoleDto.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(role);

        return MapToDto(role);
    }

    /// <summary>
    /// Deletes a role.
    /// </summary>
    public async Task DeleteRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new KeyNotFoundException($"Role with ID {id} not found");

        await _roleRepository.DeleteAsync(role);
    }

    /// <summary>
    /// Maps Role entity to RoleDto.
    /// Following Single Responsibility Principle - handles only mapping logic.
    /// </summary>
    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }
}

