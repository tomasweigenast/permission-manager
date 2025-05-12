using PermissionManager.Domain.Entities;
using PermissionManager.Domain.Interfaces;
using PermissionManager.Persistence.Contexts;

namespace PermissionManager.Persistence.Repositories;

public class PermissionTypeRepository(ApplicationDbContext dbContext) : RepositoryBase<PermissionType>(dbContext), IPermissionTypeRepository
{
    
}