using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace LeaveManagementSystem.Application.Services;

/// <summary>
/// Service implementation for user operations.
/// Following Single Responsibility Principle - handles only user-related business logic.
/// Following Open/Closed Principle - can be extended without modification through interfaces.
/// </summary>
public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;

    /// <summary>
    /// Initializes a new instance of the UserService class.
    /// Following Dependency Inversion Principle - depends on abstraction (IRepository).
    /// </summary>
    /// <param name="userRepository">Repository for user data access</param>
    /// <param name="roleRepository">Repository for role data access</param>
    public UserService(IRepository<User> userRepository, IRepository<Role> roleRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    /// <summary>
    /// Creates a new user with hashed password.
    /// </summary>
    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(createUserDto.Email))
            throw new ArgumentException("Email is required", nameof(createUserDto));

        if (string.IsNullOrWhiteSpace(createUserDto.Password))
            throw new ArgumentException("Password is required", nameof(createUserDto));

        // Check if user already exists
        var existingUser = await _userRepository.FindAsync(u => u.Email == createUserDto.Email);
        if (existingUser.Any())
            throw new InvalidOperationException("User with this email already exists");

        // Validate role exists
        var role = await _roleRepository.GetByIdAsync(createUserDto.RoleId);
        if (role == null)
            throw new ArgumentException($"Role with ID {createUserDto.RoleId} not found", nameof(createUserDto));

        // Create new user entity
        var user = new User
        {
            FullName = createUserDto.FullName,
            Email = createUserDto.Email,
            PasswordHash = HashPassword(createUserDto.Password),
            RoleId = createUserDto.RoleId,
            Status = Domain.Enums.UserStatus.Active
        };

        // Save to database
        var createdUser = await _userRepository.AddAsync(user);

        // Map to DTO
        return await MapToDtoAsync(createdUser);
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? await MapToDtoAsync(user) : null;
    }

    /// <summary>
    /// Gets a user by email.
    /// </summary>
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email);
        var user = users.FirstOrDefault();
        return user != null ? await MapToDtoAsync(user) : null;
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            userDtos.Add(await MapToDtoAsync(user));
        }
        return userDtos;
    }

    /// <summary>
    /// Updates a user.
    /// </summary>
    public async Task<UserDto> UpdateUserAsync(Guid id, CreateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        // Validate role exists if RoleId is being updated
        var role = await _roleRepository.GetByIdAsync(updateUserDto.RoleId);
        if (role == null)
            throw new ArgumentException($"Role with ID {updateUserDto.RoleId} not found", nameof(updateUserDto));

        // Update properties
        user.FullName = updateUserDto.FullName;
        user.Email = updateUserDto.Email;
        if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            user.PasswordHash = HashPassword(updateUserDto.Password);
        }
        user.RoleId = updateUserDto.RoleId;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return await MapToDtoAsync(user);
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        await _userRepository.DeleteAsync(user);
    }

    /// <summary>
    /// Validates user credentials for login.
    /// </summary>
    public async Task<UserDto?> ValidateCredentialsAsync(string email, string password)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email && u.Status == Domain.Enums.UserStatus.Active);
        var user = users.FirstOrDefault();

        if (user == null)
            return null;

        // Verify password
        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        return await MapToDtoAsync(user);
    }

    /// <summary>
    /// Maps User entity to UserDto.
    /// Following Single Responsibility Principle - handles only mapping logic.
    /// </summary>
    private async Task<UserDto> MapToDtoAsync(User user)
    {
        // Load role if not already loaded
        Role? role = null;
        if (user.Role != null)
        {
            role = user.Role;
        }
        else
        {
            role = await _roleRepository.GetByIdAsync(user.RoleId);
        }

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = role?.Name ?? string.Empty,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
    }

    /// <summary>
    /// Hashes a password using SHA256.
    /// In production, consider using bcrypt or Argon2 for better security.
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    private static bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }
}

