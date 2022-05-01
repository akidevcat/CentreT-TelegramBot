using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories.Infrastructure;

/// <inheritdoc cref="IGenericRepository{T}"/>
public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{

    protected readonly DbSet<T> DbSet;
    protected readonly DbContext DbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    public GenericRepository(DbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T> Create(T entity, bool autoSave = true)
    {
        var result = DbContext.Add(entity).Entity;
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

        var result = DbSet.Remove(entity).Entity;
        if (autoSave)
        {
            await Save();
        }

        return result;
    }

    public async Task<T?> Delete(T entity, bool autoSave = true)
    {
        var result = DbSet.Remove(entity).Entity;
        if (autoSave)
        {
            await Save();
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetFirst(params object?[]? keyValues)
    {
        return await DbSet.FindAsync(keyValues);
    }

    /// <inheritdoc />
    public async Task<T?> GetFirst(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.FirstOrDefaultAsync(predicate);
    }
    
    /// <inheritdoc />
    public Task<IQueryable<T>> Get(Expression<Func<T, bool>> predicate)
    {
        return Task.FromResult(DbSet.Where(predicate));
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
        var result = DbSet.Update(entity).Entity;
        
        if (autoSave)
        {
            await Save();
        }
        
        return result;
    }

    /// <inheritdoc />
    public virtual async Task<int> Save()
    {
        return await DbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public virtual Task<DbSet<T>> All()
    {
        return Task.FromResult(DbSet);
    }
}