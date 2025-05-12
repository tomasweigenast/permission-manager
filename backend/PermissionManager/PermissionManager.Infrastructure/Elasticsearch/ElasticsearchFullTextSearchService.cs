using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Entities;

namespace PermissionManager.Infrastructure.Elasticsearch;

public class ElasticsearchFullTextSearchService(IOptions<ElasticsearchOptions> elasticsearchOptions)
    : IFullTextSearchService
{
    private readonly ElasticsearchOptions _elasticsearchOptions = elasticsearchOptions.Value;
    private readonly ElasticsearchClient _elasticsearchClient = new(new Uri(elasticsearchOptions.Value.Uri));

    public async Task IndexPermissionAsync(Permission permission, CancellationToken ct = default)
    {
        await _elasticsearchClient.IndexAsync(permission,
            descriptor => descriptor.Index(_elasticsearchOptions.PermissionsIndexName).Id(permission.Id), ct);
    }

    public async Task DeletePermissionAsync(int id, CancellationToken ct = default)
    {
        await _elasticsearchClient.DeleteAsync<Permission>(id, ct);
    }
}