using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories.Infrastructure;

/// <inheritdoc cref="IGenericRepository{T}"/>
public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{

    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Constructor
    /// </summary>
    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T> Create(T entity)
    {
        return _dbContext.Add(entity).Entity;
    }

    /// <inheritdoc />
    public virtual async Task<T?> Delete(Guid id)
    {
        var entity = await Get(id);
        if (entity == null)
            return null;
        return _dbSet.Remove(entity).Entity;
    }

    /// <inheritdoc />
    public virtual async Task<T?> Get(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <inheritdoc />
    public virtual async Task<T?> Update(T entity)
    {
        return _dbSet.Update(entity).Entity;
    }

    /// <inheritdoc />
    public virtual async Task<int> Save()
    {
        return _dbContext.SaveChanges();
    }

    /// <inheritdoc />
    public virtual async Task<DbSet<T>> All()
    {
        return _dbSet;
    }
}