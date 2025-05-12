namespace PermissionManager.Application.DTOs;

/// <summary>
/// Represents a full snapshot of a granted permission, including employee details,
/// permission type, and the timestamp of when it was granted.
/// </summary>
/// <param name="Id">The unique identifier of the granted permission.</param>
/// <param name="EmployeeName">The first name of the employee who holds the permission.</param>
/// <param name="EmployeeSurname">The surname of the employee who holds the permission.</param>
/// <param name="PermissionType">The type of permission that was granted.</param>
/// <param name="GrantedAt">The UTC date and time the permission was granted.</param>
public readonly record struct PermissionDto(
    int Id, 
    string EmployeeName, 
    string EmployeeSurname, 
    PermissionTypeDto PermissionType,
    DateTime GrantedAt);