using Microsoft.EntityFrameworkCore;
using PermissionManager.Core.Pagination;

namespace PermissionManager.Core.Utils;

public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Asynchronously creates a <see cref="IPagedEnumerable{T}"/> from an <see cref="IQueryable{T}"/> by enumerating it asynchronously.
    /// </summary>
    /// <param name="query">The <see cref="IQueryable{T}"/> to paginate.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The amount of elements to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the async task.</param>
    public static async Task<PagedList<T>> ToPagedEnumerableAsync<T>(this IQueryable<T> query, 
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var getPageSize = pageSize + 1;
        query = query.Skip((pageNumber - 1) * pageSize).Take(getPageSize);
        var results = await query.ToListAsync(cancellationToken);

        // we have a next page
        var hasNextPage = false;
        if (results.Count == getPageSize)
        {
            results.RemoveAt(results.Count - 1);
            hasNextPage = true;
        }

        return new PagedList<T>(results, hasNextPage);
    }
}