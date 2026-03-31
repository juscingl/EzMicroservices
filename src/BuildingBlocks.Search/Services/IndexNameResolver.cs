using BuildingBlocks.Search.Abstractions;
using BuildingBlocks.Search.Options;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Search.Services;

internal sealed class IndexNameResolver(IOptions<ElasticsearchOptions> options) : IIndexNameResolver
{
    public string Resolve(string logicalName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logicalName);

        var prefix = options.Value.DefaultIndexPrefix.Trim();
        var normalized = logicalName.Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(prefix)
            ? normalized
            : $"{prefix.ToLowerInvariant()}-{normalized}";
    }
}
