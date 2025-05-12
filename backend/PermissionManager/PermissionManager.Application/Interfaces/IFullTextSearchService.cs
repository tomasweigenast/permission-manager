using PermissionManager.Domain.Entities;

namespace PermissionManager.Application.Interfaces;

/// <summary>
/// Defines operations for indexing and deleting <see cref="Permission"/> entities
/// in a full-text search index.
/// </summary>
public interface IFullTextSearchService
{
    /// <summary>
    /// Adds or updates the specified <see cref="Permission"/> in the search index,
    /// enabling it to be found via full-text queries.
    /// </summary>
    /// <param name="permission">The <see cref="Permission"/> entity to index.</param>
    /// <param name="ct">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A task representing the asynchronous indexing operation.</returns>
    Task IndexPermissionAsync(Permission permission, CancellationToken ct = default);

    /// <summary>
    /// Removes the <see cref="Permission"/> with the given identifier from the search index,
    /// so it will no longer appear in query results.
    /// </summary>
    /// <param name="id">The ID of the <see cref="Permission"/> to remove from the index.</param>
    /// <param name="ct">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A task representing the asynchronous deletion operation.</returns>
    Task DeletePermissionAsync(int id, CancellationToken ct = default);
}
