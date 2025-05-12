namespace PermissionManager.Application.DTOs;

/// <summary>
/// Describes a type of permission that can be assigned to employees.
/// </summary>
/// <param name="Id">The unique identifier of the permission type.</param>
/// <param name="Description">A human-readable description of the permission type (e.g., "Read Access").</param>
public readonly record struct PermissionTypeDto(int Id, string Description);