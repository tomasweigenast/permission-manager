namespace PermissionManager.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IPermissionRepository Permissions { get; }
    
    public IPermissionTypeRepository PermissionTypes { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    
    Task<T> RunTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}