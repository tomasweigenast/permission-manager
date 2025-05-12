using MediatR;

namespace PermissionManager.Application.Commands.ModifyPermission;

public record ModifyPermissionCommand : IRequest<int>
{
    public required int Id { get; init; }
    
    public string? EmployeeName { get; init; }
    
    public string? EmployeeSurname { get; init; }
    
    public int? PermissionTypeId { get; init; }
}