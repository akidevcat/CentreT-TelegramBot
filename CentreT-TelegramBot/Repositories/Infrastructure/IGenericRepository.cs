using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories.Infrastructure;

/// <summary>
/// Represents a generic repository with exactly one <see cref="DbSet{TEntity}"/> and CRUD operations.
/// </summary>
/// <typeparam name="T">Entity for this <see cref="DbSet{T}"/></typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">Entity to be created.</param>
    /// <param name="autoSave">Invokes <see cref="Save"/></param>
    /// <returns>Created entity</returns>
    public Task<T> Create(T entity, bool autoSave = true);

    /// <summary>
    /// Gets an entity by key values.
    /// </summary>
    /// <param name="keyValues">Key objects specifying the entity to be found.</param>
    /// <returns>First found entity. If not found, returns null.</returns>
    public Task<T?> Get(params object?[]? keyValues);
    
    /// <summary>
    /// Gets an entity by predicate.
    /// </summary>
    /// <param name="predicate">Predicate specifying the entity to be found.</param>
    /// <returns>First found entity. If not found, returns null.</returns>
    public Task<T?> Get(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Gets an entity by predicate. If not found, creates new <see cref="defaultEntity"/> entity.
    /// </summary>
    /// <param name="defaultEntity">Entity that should be placed if not found.</param>
    /// <param name="predicate">Predicate specifying the entity to be found.</param>
    /// <param name="autoSave">Invokes <see cref="Save"/>.</param>
    /// <returns>Found entity. If not found, returns created one.</returns>
    public Task<T> GetOrCreate(T defaultEntity, Expression<Func<T, bool>> predicate, bool autoSave = true);
    
    /// <summary>
    /// Gets an entity by key values. If not found, creates new <see cref="defaultEntity"/> entity.
    /// </summary>
    /// <param name="keyValues">Key objects specifying the entity to be found.</param>
    /// /// <param name="autoSave">Invokes <see cref="Save"/>.</param>
    /// <param name="defaultEntity">Entity that should be placed if not found.</param>
    /// <returns>Found entity. If not found, returns created one.</returns>
    public Task<T> GetOrCreate(T defaultEntity, bool autoSave = true, params object?[]? keyValues);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">Entity to be updated.</param>
    /// <param name="autoSave">Invokes <see cref="Save"/></param>
    /// <returns>Updated entity. If not found, returns null.</returns>
    public Task<T?> Update(T entity, bool autoSave = true);

    /// <summary>
    /// Deletes an entity by id.
    /// </summary>
    /// <param name="autoSave">Invokes <see cref="Save"/></param>
    /// <param name="keyValues">Objects specifying the entity to be deleted.</param>
    /// <returns>Deleted entity. If not found, returns null.</returns>
    public Task<T?> Delete(bool autoSave = true, params object?[]? keyValues);

    /// <summary>
    /// Saves this <see cref="DbContext"/>.
    /// </summary>
    /// <returns><inheritdoc cref="DbContext.SaveChanges()"/></returns>
    public Task<int> Save();

    /// <summary>
    /// Gets all existing tasks.
    /// </summary>
    /// <returns><see cref="DbSet{TEntity}"/> with entities.</returns>
    public Task<DbSet<T>> All();

}