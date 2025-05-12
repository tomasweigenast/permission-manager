using MediatR;
using PermissionManager.Application.DTOs;

namespace PermissionManager.Application.Commands.GetPermissionTypes;

public record GetPermissionTypesQuery : IRequest<IEnumerable<PermissionTypeDto>>
{
    
}