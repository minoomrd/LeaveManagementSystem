using System.Linq.Expressions;

namespace LeaveManagementSystem.Application.Interfaces;

/// <summary>
/// Generic repository interface following Repository Pattern.
/// Following Interface Segregation Principle - provides only essential CRUD operations.
/// Following Dependency Inversion Principle - high-level modules depend on abstraction.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its unique identifier
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Finds entities matching a predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>Collection of matching entities</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Checks if an entity exists by predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}

