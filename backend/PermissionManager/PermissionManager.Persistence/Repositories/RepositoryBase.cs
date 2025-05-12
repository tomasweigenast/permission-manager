using Microsoft.EntityFrameworkCore;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Persistence.Repositories;

public class RepositoryBase<T>(DbContext dbContext) : IRepository<T>
    where T : class
{
    protected IQueryable<T> Queryable { get; } = dbContext.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken) => await dbContext.Set<T>().FindAsync([id], cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken) => await Queryable.ToListAsync(cancellationToken);

    public virtual void Add(T entity) => dbContext.Add(entity);

    public virtual void Update(T entity) => dbContext.Update(entity);

    public virtual void Remove(T entity) => dbContext.Remove(entity);
}