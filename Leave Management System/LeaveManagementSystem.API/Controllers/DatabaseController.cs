using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Infrastructure.Data;
using Npgsql;

namespace LeaveManagementSystem.API.Controllers;

/// <summary>
/// Controller for viewing database contents via API.
/// Following Single Responsibility Principle - handles only database viewing operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the DatabaseController class.
    /// </summary>
    /// <param name="context">Database context</param>
    public DatabaseController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all users from the database.
    /// </summary>
    /// <returns>Collection of users</returns>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            // Check connection first to avoid retry delays
            if (!await _context.Database.CanConnectAsync())
            {
                return StatusCode(503, new { 
                    error = "Database not available", 
                    message = "PostgreSQL is not running or not installed. Please install PostgreSQL and ensure it's running.",
                    help = "Run INSTALL-POSTGRESQL.bat to install PostgreSQL"
                });
            }

            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    u.Status,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Npgsql.NpgsqlException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
        {
            return StatusCode(503, new { 
                error = "Database connection refused", 
                message = "PostgreSQL is not running. Please start PostgreSQL service.",
                help = "PostgreSQL needs to be installed and running on port 5432"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Database error", message = ex.Message });
        }
    }

    /// <summary>
    /// Gets all companies from the database.
    /// </summary>
    /// <returns>Collection of companies</returns>
    [HttpGet("companies")]
    public async Task<IActionResult> GetCompanies()
    {
        try
        {
            if (!await _context.Database.CanConnectAsync())
            {
                return StatusCode(503, new { 
                    error = "Database not available", 
                    message = "PostgreSQL is not running or not installed."
                });
            }

            var companies = await _context.Companies
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ContactEmail,
                    c.OwnerId,
                    c.Status,
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(companies);
        }
        catch (NpgsqlException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
        {
            return StatusCode(503, new { error = "Database connection refused", message = "PostgreSQL is not running." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Database error", message = ex.Message });
        }
    }

    /// <summary>
    /// Gets all leave requests from the database.
    /// </summary>
    /// <returns>Collection of leave requests</returns>
    [HttpGet("leave-requests")]
    public async Task<IActionResult> GetLeaveRequests()
    {
        try
        {
            if (!await _context.Database.CanConnectAsync())
            {
                return StatusCode(503, new { error = "Database not available", message = "PostgreSQL is not running or not installed." });
            }

            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.User)
                .Include(lr => lr.LeaveType)
                .Select(lr => new
                {
                    lr.Id,
                    UserName = lr.User.FullName,
                    LeaveTypeName = lr.LeaveType.Name,
                    lr.StartDateTime,
                    lr.EndDateTime,
                    lr.DurationAmount,
                    lr.DurationUnit,
                    lr.Status,
                    lr.Reason,
                    lr.CreatedAt
                })
                .ToListAsync();

            return Ok(leaveRequests);
        }
        catch (NpgsqlException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
        {
            return StatusCode(503, new { error = "Database connection refused", message = "PostgreSQL is not running." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Database error", message = ex.Message });
        }
    }

    /// <summary>
    /// Gets all leave balances from the database.
    /// </summary>
    /// <returns>Collection of leave balances</returns>
    [HttpGet("leave-balances")]
    public async Task<IActionResult> GetLeaveBalances()
    {
        try
        {
            if (!await _context.Database.CanConnectAsync())
            {
                return StatusCode(503, new { error = "Database not available", message = "PostgreSQL is not running or not installed." });
            }

            var leaveBalances = await _context.LeaveBalances
                .Include(lb => lb.User)
                .Include(lb => lb.LeaveType)
                .Select(lb => new
                {
                    lb.Id,
                    UserName = lb.User.FullName,
                    LeaveTypeName = lb.LeaveType.Name,
                    lb.BalanceAmount,
                    lb.BalanceUnit,
                    lb.UpdatedAt
                })
                .ToListAsync();

            return Ok(leaveBalances);
        }
        catch (NpgsqlException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
        {
            return StatusCode(503, new { error = "Database connection refused", message = "PostgreSQL is not running." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Database error", message = ex.Message });
        }
    }

    /// <summary>
    /// Gets database connection status.
    /// </summary>
    /// <returns>Connection status</returns>
    [HttpGet("status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var dbName = _context.Database.GetDbConnection().Database;
            var server = _context.Database.GetDbConnection().DataSource;

            return Ok(new
            {
                connected = canConnect,
                database = dbName,
                server = server,
                message = canConnect ? "Database is connected" : "Database connection failed"
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                connected = false,
                message = "Database not available",
                error = ex.Message
            });
        }
    }
}

