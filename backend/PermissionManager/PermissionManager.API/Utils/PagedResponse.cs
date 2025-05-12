namespace PermissionManager.API.Utils;

/// <summary>
/// A class that is used to return paginated lists
/// </summary>
public record PagedResponse<T>
{
    public required IEnumerable<T> Data { get; init; }
    
    public required bool HasNextPage { get; init; }
}