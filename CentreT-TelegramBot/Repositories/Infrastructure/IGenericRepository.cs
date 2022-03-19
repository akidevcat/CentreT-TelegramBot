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
    /// <returns>Created entity</returns>
    public Task<T> Create(T entity);

    /// <summary>
    /// Gets an entity by id.
    /// </summary>
    /// <param name="id"><see cref="Guid"/> specifying the entity to be found.</param>
    /// <returns>Found entity. If not found, returns null.</returns>
    public Task<T?> Get(Guid id);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">Entity to be updated.</param>
    /// <returns>Updated entity. If not found, returns null.</returns>
    public Task<T?> Update(T entity);

    /// <summary>
    /// Deletes an entity by id.
    /// </summary>
    /// <param name="id"><see cref="Guid"/> specifying the entity to be deleted.</param>
    /// <returns>Deleted entity. If not found, returns null.</returns>
    public Task<T?> Delete(Guid id);

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