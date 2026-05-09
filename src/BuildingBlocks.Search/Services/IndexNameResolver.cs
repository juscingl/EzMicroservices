using BuildingBlocks.Search.Abstractions;
using BuildingBlocks.Search.Options;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Search.Services;

internal sealed class IndexNameResolver(IOptions<ElasticsearchOptions> options) : IIndexNameResolver
{
    /// <summary>
    /// 根据默认前缀和逻辑名称生成标准索引名（统一小写）。
    /// </summary>
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
