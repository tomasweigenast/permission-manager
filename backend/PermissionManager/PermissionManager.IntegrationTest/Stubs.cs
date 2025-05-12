using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Entities;

namespace PermissionManager.IntegrationTest;

public class NoOpFullTextSearch : IFullTextSearchService
{
    public Task IndexPermissionAsync(Permission permission, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task DeletePermissionAsync(int id, CancellationToken ct = default)
        => Task.CompletedTask;
}
public class NoOpProducerService : IProducerService
{
    public Task ProduceAsync(OperationDto dto, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}