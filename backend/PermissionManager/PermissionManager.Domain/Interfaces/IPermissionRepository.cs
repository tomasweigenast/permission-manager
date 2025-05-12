using PermissionManager.Core.Pagination;
using PermissionManager.Domain.Entities;

namespace PermissionManager.Domain.Interfaces;

/// <summary>
/// Represents a repository for managing permissions.
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>
    /// Gets a paginated list of permissions asynchronously.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="permissionTypeId">Optional filter for permission type.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of permissions.</returns>
    public Task<PagedList<Permission>> GetAllAsync(
        int pageSize,
        int page,
        int? permissionTypeId,
        CancellationToken cancellationToken = default);
}