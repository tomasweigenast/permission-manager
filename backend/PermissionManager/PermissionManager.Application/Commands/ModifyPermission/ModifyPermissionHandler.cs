using MediatR;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.ModifyPermission;

public class ModifyPermissionHandler(IUnitOfWork unitOfWork, IFullTextSearchService ftsService, IProducerService producerService) : IRequestHandler<ModifyPermissionCommand, int>
{
    public async Task<int> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Permissions.GetByIdAsync(request.Id, cancellationToken);
        if (existing == null) throw new KeyNotFoundException($"Permission {request.Id} not found");
        
        if(!string.IsNullOrWhiteSpace(request.EmployeeName))
            existing.EmployeeName = request.EmployeeName;
        
        if(!string.IsNullOrWhiteSpace(request.EmployeeSurname))
            existing.EmployeeSurname = request.EmployeeSurname;
        
        if(request.PermissionTypeId.HasValue)
            existing.PermissionTypeId = request.PermissionTypeId.Value;

        // implement a more durable, robust way of doing this
        // maybe circuit-breaker? outbox pattern? 
        await unitOfWork.RunTransactionAsync(async (ct) =>
        {
            // save to sql server
            unitOfWork.Permissions.Update(existing);
            await unitOfWork.CommitAsync(ct);

            // index in fts
            // if this fails, it will revert sql server
            await ftsService.IndexPermissionAsync(existing, cancellationToken);
            
            // publish event
            try
            {
                // if this fails, it will revert sql server, but we still need to revert fts
                await producerService.ProduceAsync(
                    new OperationDto { Id = Guid.NewGuid(), OperationName = OperationType.Modify }, cancellationToken);
            }
            catch
            {
                // delete from fts
                await ftsService.DeletePermissionAsync(existing.Id, cancellationToken);
                
                // throw to revert sql server
                throw;
            }

            return 1;
        }, cancellationToken);
        return existing.Id;
    }
}