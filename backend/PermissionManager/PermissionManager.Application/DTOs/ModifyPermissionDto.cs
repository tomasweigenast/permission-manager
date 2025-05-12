namespace PermissionManager.Application.DTOs;

/// <summary>
/// Represents a request to modify an existing permission,
/// allowing partial updates to employee name, surname, or permission type.
/// </summary>
/// <param name="EmployeeName">The updated first name of the employee, if applicable.</param>
/// <param name="EmployeeSurname">The updated surname of the employee, if applicable.</param>
/// <param name="PermissionTypeId">The new permission type ID to apply, if changed.</param>
public readonly record struct ModifyPermissionDto(
    string? EmployeeName,
    string? EmployeeSurname,
    int? PermissionTypeId);