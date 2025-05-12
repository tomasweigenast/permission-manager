namespace PermissionManager.Infrastructure.Elasticsearch;

/// <summary>
/// Represents configuration options for connecting to Elasticsearch.
/// </summary>
public class ElasticsearchOptions
{
    /// <summary>
    /// The URI of the Elasticsearch server.
    /// </summary>
    public required string Uri { get; init; }
    
    /// <summary>
    /// The name of the index for permissions.
    /// </summary>
    public required string PermissionsIndexName { get; init; }
}