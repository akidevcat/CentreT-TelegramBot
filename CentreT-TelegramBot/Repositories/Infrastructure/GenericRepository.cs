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
        var entity = await GetFirst(keyValues);
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

    public async Task Delete(T entity, bool autoSave = true)
    {
        _dbSet.Remove(entity);
        if (autoSave)
        {
            await Save();
        }
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetFirst(params object?[]? keyValues)
    {
        return await _dbSet.FindAsync(keyValues);
    }

    /// <inheritdoc />
    public async Task<T?> GetFirst(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
    
    /// <inheritdoc />
    public Task<IQueryable<T>> Get(Expression<Func<T, bool>> predicate)
    {
        return Task.FromResult(_dbSet.Where(predicate));
    }

    /// <inheritdoc />
    public async Task<T> GetOrCreate(T defaultEntity, Expression<Func<T, bool>> predicate, bool autoSave = true)
    {
        var result = await GetFirst(predicate) ?? await Create(defaultEntity);
        
        if (autoSave)
        {
            await Save();
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T> GetOrCreate(T defaultEntity, bool autoSave = true, params object?[]? keyValues)
    {
        var result = await GetFirst(keyValues) ?? await Create(defaultEntity);

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