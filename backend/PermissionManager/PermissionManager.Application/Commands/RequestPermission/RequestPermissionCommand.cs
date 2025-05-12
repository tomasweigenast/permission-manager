using MediatR;

namespace PermissionManager.Application.Commands.RequestPermission;

public record RequestPermissionCommand : IRequest<int>
{
    public required string EmployeeName { get; init; }
    
    public required string EmployeeSurname { get; init; }
    
    public required int PermissionTypeId { get; init; }
}