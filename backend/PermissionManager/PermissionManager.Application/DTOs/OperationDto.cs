using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PermissionManager.Application.DTOs;

public readonly record struct OperationDto(Guid Id, OperationType OperationName);

public enum OperationType
{
    [JsonStringEnumMemberName("modify")]
    Modify,
    
    [JsonStringEnumMemberName("request")]
    Request,
    
    [JsonStringEnumMemberName("get")]
    Get
}