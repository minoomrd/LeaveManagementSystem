using LeaveManagementSystem.Application.DTOs;

namespace LeaveManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for user operations.
/// Following Interface Segregation Principle - defines only user-related operations.
/// Following Dependency Inversion Principle - high-level modules depend on abstraction.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="createUserDto">User creation data</param>
    /// <returns>Created user DTO</returns>
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User DTO if found, null otherwise</returns>
    Task<UserDto?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>User DTO if found, null otherwise</returns>
    Task<UserDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>Collection of user DTOs</returns>
    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateUserDto">User update data</param>
    /// <returns>Updated user DTO</returns>
    Task<UserDto> UpdateUserAsync(Guid id, CreateUserDto updateUserDto);

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">User ID</param>
    Task DeleteUserAsync(Guid id);

    /// <summary>
    /// Validates user credentials for login
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="password">Plain text password</param>
    /// <returns>User DTO if credentials are valid, null otherwise</returns>
    Task<UserDto?> ValidateCredentialsAsync(string email, string password);
}

