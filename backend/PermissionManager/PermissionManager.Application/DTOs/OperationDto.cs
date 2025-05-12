using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PermissionManager.Application.DTOs;

/// <summary>
/// Represents an operation event to be published.
/// </summary>
/// <param name="Id">The unique identifier for the entity affected by the operation.</param>
/// <param name="OperationName">The type of operation performed (e.g., modify, request, get).</param>
public readonly record struct OperationDto(Guid Id, OperationType OperationName);

/// <summary>
/// Enumerates the types of operations that can be performed on a permission.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Indicates that a permission was modified.
    /// </summary>
    [JsonStringEnumMemberName("modify")]
    Modify,

    /// <summary>
    /// Indicates that a new permission was requested.
    /// </summary>
    [JsonStringEnumMemberName("request")]
    Request,

    /// <summary>
    /// Indicates that a permission or permissions were retrieved.
    /// </summary>
    [JsonStringEnumMemberName("get")]
    Get
}