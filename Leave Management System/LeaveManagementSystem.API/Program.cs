using LeaveManagementSystem.Application;
using LeaveManagementSystem.Infrastructure;
using LeaveManagementSystem.Infrastructure.Data;
using LeaveManagementSystem.API.Converters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container
// Following Dependency Inversion Principle - register dependencies through DI container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Convert property names to camelCase for JavaScript compatibility
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Allow flexible date parsing and enum conversion
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Add custom DateTime converter for ISO 8601 format
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        // Allow case-insensitive property matching
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Leave Management System API",
        Version = "v1",
        Description = "API for managing employee leave requests, balances, and company subscriptions"
    });
    
    // Use camelCase for Swagger schema to match JSON serialization
    c.CustomSchemaIds(type => type.Name);
    
    // Map enum values as strings in Swagger
    c.MapType<DateTime>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "date-time",
        Example = new Microsoft.OpenApi.Any.OpenApiString("2025-12-25T23:00:00.000Z")
    });
});

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Application layer services
builder.Services.AddApplication();

// Add Infrastructure layer services (Database, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Get logger for diagnostics
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application starting...");
logger.LogInformation("Swagger will be available at: /swagger");

// Configure the HTTP request pipeline
// Enable Swagger in all environments for easier testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leave Management System API v1");
    c.RoutePrefix = "swagger";
});

logger.LogInformation("Swagger middleware configured");

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Enable routing and controllers
app.UseRouting();
app.MapControllers();

// Simple health check endpoint
app.MapGet("/", () => Results.Ok(new { message = "API is running", swagger = "/swagger" })).ExcludeFromDescription();

// Diagnostic endpoint to check Swagger
app.MapGet("/api/diagnostics", () => 
{
    return Results.Ok(new 
    { 
        message = "API Diagnostics",
        swaggerJson = "/swagger/v1/swagger.json",
        swaggerUI = "/swagger",
        timestamp = DateTime.UtcNow
    });
}).ExcludeFromDescription();

logger.LogInformation("Application configured. Controllers mapped.");

// Apply migrations and seed database on startup
// Run asynchronously so it doesn't block startup
_ = Task.Run(async () =>
{
    await Task.Delay(3000); // Wait for app to fully start
    try
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        try
        {
            if (await context.Database.CanConnectAsync())
            {
                // Apply pending migrations
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
                
                // Seed database
                await SeedData.SeedAsync(context);
                logger.LogInformation("Database seeded successfully.");
            }
            else
            {
                logger.LogWarning("Database not available. API is running but database operations will fail. Install PostgreSQL and ensure it's running.");
            }
        }
        catch (Exception dbEx)
        {
            // Database connection failed - API still works
            logger.LogError($"Database error: {dbEx.Message}. API is running but database features are disabled.");
            if (dbEx.InnerException != null)
            {
                logger.LogError($"Inner exception: {dbEx.InnerException.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        // Don't fail startup if database is unavailable
        Console.WriteLine($"Database initialization skipped: {ex.Message}");
    }
});

app.Run();
