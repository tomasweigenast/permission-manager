using MediatR;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Entities;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.RequestPermission;

/// <summary>
/// Handles the creation of a new permission request.
/// <param name="unitOfWork">The unit of work for data access.</param>
/// <param name="ftsService">The full-text search service for indexing.</param>
/// <param name="producerService">The service for producing operation events.</param>
/// </summary>
public class RequestPermissionHandler(IUnitOfWork unitOfWork, IFullTextSearchService ftsService, IProducerService producerService) : IRequestHandler<RequestPermissionCommand, int>
{
    /// <summary>
    /// Handles the RequestPermissionCommand request.
    /// <param name="request">The command containing permission request details.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>The ID of the newly created permission.</returns>
    /// </summary>
    public async Task<int> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
    {
        var entity = new Permission
        {
            EmployeeName = request.EmployeeName,
            EmployeeSurname = request.EmployeeSurname,
            PermissionTypeId = request.PermissionTypeId,
            CreatedAt = DateTime.UtcNow
        };

        // implement a more durable, robust way of doing this
        // maybe circuit-breaker? outbox pattern? 
        await unitOfWork.RunTransactionAsync(async (ct) =>
        {
            // Save the permissions
            unitOfWork.Permissions.Add(entity);
            await unitOfWork.CommitAsync(ct);
            
            // index in fts
            // if this fails, it will revert sql server
            await ftsService.IndexPermissionAsync(entity, cancellationToken);
            
            // publish event
            try
            {
                // if this fails, it will revert sql server, but we still need to revert fts
                await producerService.ProduceAsync(
                    new OperationDto { Id = Guid.NewGuid(), OperationName = OperationType.Request }, cancellationToken);
            }
            catch
            {
                // delete from fts
                await ftsService.DeletePermissionAsync(entity.Id, cancellationToken);
                
                // throw to revert sql server
                throw;
            }

            return 1;
        }, cancellationToken);

        return entity.Id;
    }
}