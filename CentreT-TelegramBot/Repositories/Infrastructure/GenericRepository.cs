using System.Linq.Expressions;
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
    public virtual async Task<T> Create(T entity, bool autoSave = true)
    {
        var result = _dbContext.Add(entity).Entity;
        if (autoSave)
        {
            await Save();
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> Delete(bool autoSave = true, params object?[]? keyValues)
    {
        var entity = await Get(keyValues);
        if (entity == null)
        {
            return null;
        }

        var result = _dbSet.Remove(entity).Entity;
        if (autoSave)
        {
            await Save();
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> Get(params object?[]? keyValues)
    {
        return await _dbSet.FindAsync(keyValues);
    }

    /// <inheritdoc />
    public async Task<T?> Get(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    /// <inheritdoc />
    public async Task<T> GetOrCreate(T defaultEntity, Expression<Func<T, bool>> predicate, bool autoSave = true)
    {
        var result = await Get(predicate) ?? await Create(defaultEntity);
        
        if (autoSave)
        {
            await Save();
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T> GetOrCreate(T defaultEntity, bool autoSave = true, params object?[]? keyValues)
    {
        var result = await Get(keyValues) ?? await Create(defaultEntity);

        if (autoSave)
        {
            await Save();
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> Update(T entity, bool autoSave = true)
    {
        var result = _dbSet.Update(entity).Entity;
        
        if (autoSave)
        {
            await Save();
        }
        
        return result;
    }

    /// <inheritdoc />
    public virtual async Task<int> Save()
    {
        return await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public virtual Task<DbSet<T>> All()
    {
        return Task.FromResult(_dbSet);
    }
}