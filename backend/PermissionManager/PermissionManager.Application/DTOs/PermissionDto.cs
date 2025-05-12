namespace PermissionManager.Application.DTOs;

public readonly record struct PermissionDto(
    int Id, 
    string EmployeeName, 
    string EmployeeSurname, 
    PermissionTypeDto PermissionType,
    DateTime GrantedAt);