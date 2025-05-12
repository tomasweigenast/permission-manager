using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.GetPermission;

/// <summary>
/// Handles the retrieval of a single permission by its ID.
/// <param name="unitOfWork">The unit of work for data access.</param>
/// <param name="mapper">The AutoMapper instance for mapping entities.</param>
/// <param name="producerService">The service for producing operation events.</param>
/// <param name="logger">Logger for logging errors and information.</param>
/// </summary>
public class GetPermissionHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService, ILogger<GetPermissionHandler> logger) : IRequestHandler<GetPermissionQuery, PermissionDto?>
{
    /// <summary>
    /// Handles the GetPermissionQuery request.
    /// <param name="request">The query containing the permission ID.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>The permission DTO if found, otherwise null.</returns>
    /// </summary>
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