namespace PermissionManager.Application.DTOs;

public readonly record struct RequestPermissionDto(
    string EmployeeName,
    string EmployeeSurname,
    int PermissionTypeId);