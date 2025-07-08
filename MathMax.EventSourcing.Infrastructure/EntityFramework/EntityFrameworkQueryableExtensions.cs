using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MathMax.EventSourcing.Infrastructure.EntityFramework;

public static class EntityFrameworkQueryableExtensions
{
    public static async Task<ImmutableArray<T>> ToImmutableArrayAsync<T>(
        this IQueryable<T> source,
        CancellationToken cancellationToken = default)
    {
        var builder = ImmutableArray.CreateBuilder<T>();

        await foreach (var item in source.AsAsyncEnumerable().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            builder.Add(item);
        }

        return builder.ToImmutable();
    }
}