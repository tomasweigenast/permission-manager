using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Core.Pagination;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.GetPermissions;

public class GetPermissionsHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService,
    ILogger<GetPermissionsHandler> logger) : IRequestHandler<GetPermissionsQuery, PagedList<PermissionDto>>
{
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