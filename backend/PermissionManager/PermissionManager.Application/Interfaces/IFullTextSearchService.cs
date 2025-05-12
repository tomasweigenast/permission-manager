using PermissionManager.Domain.Entities;

namespace PermissionManager.Application.Interfaces;

public interface IFullTextSearchService
{
    Task IndexPermissionAsync(Permission permission, CancellationToken ct = default);
    
    Task DeletePermissionAsync(int id, CancellationToken ct = default);
}