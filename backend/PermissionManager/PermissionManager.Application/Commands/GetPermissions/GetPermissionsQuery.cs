using MediatR;
using PermissionManager.Application.DTOs;
using PermissionManager.Core.Pagination;

namespace PermissionManager.Application.Commands.GetPermissions;

public record GetPermissionsQuery : IRequest<PagedList<PermissionDto>>
{
    public required int PageSize { get; set; }
    
    public required int Page { get; set; }
    
    public int? PermissionTypeId { get; set; }
}