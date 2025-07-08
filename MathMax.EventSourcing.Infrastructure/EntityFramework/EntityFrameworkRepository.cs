using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MathMax.EventSourcing.Core.Repositories;

namespace MathMax.EventSourcing.Infrastructure.EntityFramework;

public class EntityFrameworkRepository<T>(DbContext context) : EntityFrameworkRepository<T, int>(context)
    where T : class
{
}

public class EntityFrameworkRepository<T, TKey> : IAsyncRepository<T, TKey> 
    where T : class 
    where TKey : struct
{
    public EntityFrameworkRepository(DbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
    }

    protected DbContext Context { get; }

    public virtual async Task DeleteAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(TKey id)
    {
        var entity = await FindByIdAsync(id);

        if (entity == null)
        {
            throw EntityNotFoundException.ForEntity<T>(id);
        }

        await DeleteAsync(entity);
    }

    public virtual async Task<T?> FindByIdAsync(TKey id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public virtual async Task InsertAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public virtual Task<ImmutableArray<T>> AllAsync()
    {
        return Context.Set<T>().ToImmutableArrayAsync();
    }

    public virtual Task<ImmutableArray<T>> AllWhereAsync(Expression<Func<T, bool>> expression)
    {
        return Context.Set<T>().Where(expression).ToImmutableArrayAsync();
    }

    public virtual async Task<IQueryable<T>> AllQueryableWhereAsync(Expression<Func<T, bool>> expression)
    {
        return await Task.FromResult(Context.Set<T>().Where(expression));
    }

    public virtual async Task ReloadAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Context.Entry(entity).ReloadAsync();
    }

    public virtual async Task RemoveRangeAsync(ICollection<T> objectsToRemove)
    {
        Context.Set<T>().RemoveRange(objectsToRemove);
        await Context.SaveChangesAsync();
    }

    public virtual Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Context.Set<T>().Update(entity);
        return Context.SaveChangesAsync();
    }
}


