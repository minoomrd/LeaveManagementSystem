using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LeaveManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation using Entity Framework Core.
/// Following Single Responsibility Principle - handles only data access operations.
/// Following Open/Closed Principle - can be extended through inheritance.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// Following Dependency Inversion Principle - depends on ApplicationDbContext abstraction.
    /// </summary>
    /// <param name="context">Database context</param>
    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Gets all entities.
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Finds entities matching a predicate.
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    public virtual async Task UpdateAsync(T entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Detached)
        {
            // Entity is not tracked, attach it first
            _dbSet.Attach(entity);
        }
        
        // Mark all properties as modified except the primary key (Id)
        entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        
        // Explicitly exclude primary key properties from being marked as modified
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey != null)
        {
            foreach (var keyProperty in primaryKey.Properties)
            {
                entry.Property(keyProperty.Name).IsModified = false;
            }
        }
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if an entity exists by predicate.
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}

