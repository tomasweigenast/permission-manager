namespace PermissionManager.API.Utils;

public class PagedResponse<T>
{
    public required IEnumerable<T> Data { get; init; }
    
    public required bool HasNextPage { get; init; }
}