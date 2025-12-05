using LeaveManagementSystem.Application.DTOs;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

/// <summary>
/// Controller for authentication operations.
/// Following Single Responsibility Principle - handles only authentication-related HTTP requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the AuthController class.
    /// Following Dependency Inversion Principle - depends on IUserService abstraction.
    /// </summary>
    /// <param name="userService">User service</param>
    public AuthController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <summary>
    /// Authenticates a user and returns user information.
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>User DTO if authentication succeeds</returns>
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userService.ValidateCredentialsAsync(loginDto.Email, loginDto.Password);
        if (user == null)
            return Unauthorized("Invalid email or password");

        return Ok(user);
    }
}

