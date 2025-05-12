using PermissionManager.Domain.Interfaces;
using PermissionManager.Persistence.Contexts;

namespace PermissionManager.Persistence.UnitOfWork;

public class UnitOfWork(ApplicationDbContext dbContext, IPermissionRepository permissionRepository, IPermissionTypeRepository permissionTypeRepository) : IUnitOfWork
{
    public IPermissionRepository Permissions => permissionRepository;
    public IPermissionTypeRepository PermissionTypes => permissionTypeRepository;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        dbContext.Dispose();
    }
    
    public async Task<int> CommitAsync(CancellationToken cancellationToken) => await dbContext.SaveChangesAsync(cancellationToken);
    
    public async Task<T> RunTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}