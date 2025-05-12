namespace PermissionManager.Application.DTOs;

/// <summary>
/// Represents a client-side request to create a new permission.
/// </summary>
/// <param name="EmployeeName">The first name of the employee requesting the permission.</param>
/// <param name="EmployeeSurname">The surname of the employee requesting the permission.</param>
/// <param name="PermissionTypeId">The ID of the permission type being requested.</param>
public readonly record struct RequestPermissionDto(
    string EmployeeName,
    string EmployeeSurname,
    int PermissionTypeId);