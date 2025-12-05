namespace LeaveManagementSystem.Domain.Common;

/// <summary>
/// Base entity class that provides common properties for all domain entities.
/// Following DRY (Don't Repeat Yourself) principle - common properties are centralized.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity (Primary Key)
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

