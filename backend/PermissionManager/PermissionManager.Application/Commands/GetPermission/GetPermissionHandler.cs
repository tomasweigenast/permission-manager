using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.GetPermission;

public class GetPermissionHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService, ILogger<GetPermissionHandler> logger) : IRequestHandler<GetPermissionQuery, PermissionDto?>
{
    public async Task<PermissionDto?> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
    {
        var permission = await unitOfWork.Permissions.GetByIdAsync(request.Id, cancellationToken);
        if (permission == null) return null;
        
        // publish an event
        try
        {
            await producerService.ProduceAsync(new OperationDto
            {
                Id = Guid.NewGuid(),
                OperationName = OperationType.Get
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not produce an operation message");
        }
        
        return mapper.Map<PermissionDto>(permission);
    }
}