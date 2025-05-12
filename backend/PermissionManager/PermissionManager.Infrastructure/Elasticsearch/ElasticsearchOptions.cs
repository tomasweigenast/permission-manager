namespace PermissionManager.Infrastructure.Elasticsearch;

public class ElasticsearchOptions
{
    public required string Uri { get; init; }
    
    public required string PermissionsIndexName { get; init; }
}