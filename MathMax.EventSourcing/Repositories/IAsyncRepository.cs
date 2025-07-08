using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MathMax.EventSourcing.Core.Repositories;

public interface IAsyncRepository<T> : IAsyncRepository<T, int> 
    where T : class
{
}

public interface IAsyncRepository<T, TKey> 
    where T : class
    where TKey : struct
{
    Task DeleteAsync(T entity);

    Task DeleteAsync(TKey id);

    Task<T?> FindByIdAsync(TKey id);

    Task InsertAsync(T entity);

    Task<ImmutableArray<T>> AllAsync();

    Task<ImmutableArray<T>> AllWhereAsync(Expression<Func<T, bool>> expression);

    Task<IQueryable<T>> AllQueryableWhereAsync(Expression<Func<T, bool>> expression);

    Task UpdateAsync(T entity);

    Task ReloadAsync(T entity);

    Task RemoveRangeAsync(ICollection<T> objectsToRemove);
}
