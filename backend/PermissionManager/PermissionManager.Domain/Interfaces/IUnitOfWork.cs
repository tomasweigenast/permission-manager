namespace PermissionManager.Domain.Interfaces;

/// <summary>
/// Represents a unit of work that coordinates the writing of changes and manages repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the permission repository.
    /// </summary>
    public IPermissionRepository Permissions { get; }

    /// <summary>
    /// Gets the permission type repository.
    /// </summary>
    public IPermissionTypeRepository PermissionTypes { get; }

    /// <summary>
    /// Commits all changes made in the current unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs the specified action within a transaction asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the action.</typeparam>
    /// <param name="action">The action to execute within the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result of the action.</returns>
    Task<T> RunTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}