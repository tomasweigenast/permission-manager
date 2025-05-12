using PermissionManager.Core.Pagination;
using PermissionManager.Domain.Entities;

namespace PermissionManager.Domain.Interfaces;

public interface IPermissionRepository : IRepository<Permission>
{
    public Task<PagedList<Permission>> GetAllAsync(
        int pageSize,
        int page,
        int? permissionTypeId,
        CancellationToken cancellationToken = default);
}