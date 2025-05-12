using System.Collections;

namespace PermissionManager.Core.Pagination;

/// <summary>
/// An implementation of <see cref="IEnumerable{T}"/> which contains paging information
/// </summary>
public class PagedList<T>(IEnumerable<T> items, bool hasNextPage = false) : IEnumerable<T>
{
    /// A flag that indicates if we have a next page
    public bool HasNextPage { get; } = hasNextPage;

    public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}