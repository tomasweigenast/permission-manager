using MediatR;
using PermissionManager.Application.DTOs;

namespace PermissionManager.Application.Commands.GetPermission;

public record GetPermissionQuery : IRequest<PermissionDto?>
{
    public required int Id { get; init; }
}