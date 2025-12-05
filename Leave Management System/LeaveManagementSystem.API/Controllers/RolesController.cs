using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

/// <summary>
/// Controller for role management operations.
/// Following Single Responsibility Principle - handles only role-related HTTP requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// Initializes a new instance of the RolesController class.
    /// Following Dependency Inversion Principle - depends on IRoleService abstraction.
    /// </summary>
    /// <param name="roleService">Role service</param>
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>Collection of roles</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Gets a role by ID.
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role DTO</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound($"Role with ID {id} not found");

        return Ok(role);
    }

    /// <summary>
    /// Gets a role by name.
    /// </summary>
    /// <param name="name">Role name</param>
    /// <returns>Role DTO</returns>
    [HttpGet("name/{name}")]
    public async Task<ActionResult<RoleDto>> GetRoleByName(string name)
    {
        var role = await _roleService.GetRoleByNameAsync(name);
        if (role == null)
            return NotFound($"Role with name '{name}' not found");

        return Ok(role);
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="createRoleDto">Role creation data</param>
    /// <returns>Created role DTO</returns>
    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            var role = await _roleService.CreateRoleAsync(createRoleDto);
            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            return StatusCode(500, new { error = "Database error", message = dbEx.InnerException?.Message ?? dbEx.Message });
        }
        catch (Npgsql.NpgsqlException npgsqlEx)
        {
            return StatusCode(500, new { error = "Database connection error", message = "Cannot connect to PostgreSQL. Please ensure PostgreSQL is running and the database exists.", details = npgsqlEx.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Updates a role.
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="updateRoleDto">Role update data</param>
    /// <returns>Updated role DTO</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(Guid id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        try
        {
            var role = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            return Ok(role);
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
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        try
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

