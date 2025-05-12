namespace PermissionManager.Application.DTOs;

public readonly record struct ModifyPermissionDto(
    string? EmployeeName,
    string? EmployeeSurname,
    int? PermissionTypeId);