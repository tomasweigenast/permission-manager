using Microsoft.EntityFrameworkCore;
using PermissionManager.Core.Pagination;
using PermissionManager.Core.Utils;
using PermissionManager.Domain.Entities;
using PermissionManager.Domain.Interfaces;
using PermissionManager.Persistence.Contexts;

namespace PermissionManager.Persistence.Repositories;

public class PermissionRepository(ApplicationDbContext dbContext) : RepositoryBase<Permission>(dbContext), IPermissionRepository
{
    public override async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => await Queryable.Include(x => x.PermissionType).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public override async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken)
        => await Queryable.Include(x => x.PermissionType).ToListAsync(cancellationToken);

    public async Task<PagedList<Permission>> GetAllAsync(int pageSize, int page, int? permissionTypeId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Permission> query = Queryable
            .Include(x => x.PermissionType)
            .OrderBy(x => x.EmployeeName);
        
        if(permissionTypeId != null)
            query = query.Where(x => x.PermissionTypeId == permissionTypeId);
        
        return await query.ToPagedEnumerableAsync(
            pageNumber: page,
            pageSize: pageSize,
            cancellationToken);
    }
}