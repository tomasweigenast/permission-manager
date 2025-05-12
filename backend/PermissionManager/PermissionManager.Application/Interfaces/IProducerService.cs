using PermissionManager.Application.DTOs;

namespace PermissionManager.Application.Interfaces;

public interface IProducerService
{
    Task ProduceAsync(OperationDto operation, CancellationToken ct = default);
}