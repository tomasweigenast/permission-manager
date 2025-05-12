using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Core.Pagination;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.GetPermissions;

/// <summary>
/// Handles the retrieval of a paginated list of permissions.
/// <param name="unitOfWork">The unit of work for data access.</param>
/// <param name="mapper">The AutoMapper instance for mapping entities.</param>
/// <param name="producerService">The service for producing operation events.</param>
/// <param name="logger">Logger for logging errors and information.</param>
/// </summary>
public class GetPermissionsHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService,
    ILogger<GetPermissionsHandler> logger) : IRequestHandler<GetPermissionsQuery, PagedList<PermissionDto>>
{
    /// <summary>
    /// Handles the GetPermissionsQuery request.
    /// <param name="request">The query containing pagination and filter parameters.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>A paginated list of permission DTOs.</returns>
    /// </summary>
    public async Task<PagedList<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var list = await unitOfWork
            .Permissions
            .GetAllAsync(
                pageSize: request.PageSize,
                page: request.Page,
                permissionTypeId: request.PermissionTypeId,
                cancellationToken);
        
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
            logger.LogError(ex, "Unable to produce operation.");   
        }
        
        return mapper.Map<PagedList<PermissionDto>>(list);
    }
}