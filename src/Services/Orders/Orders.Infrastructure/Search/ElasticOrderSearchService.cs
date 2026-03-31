using System.Net;
using System.Text;
using System.Text.Json;
using BuildingBlocks.Search.Abstractions;
using BuildingBlocks.Search.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orders.Application.Search;
using Orders.Domain.Entities;
using Orders.Infrastructure.Search.Documents;

namespace Orders.Infrastructure.Search;

internal sealed class ElasticOrderSearchService(
    IHttpClientFactory httpClientFactory,
    IIndexNameResolver indexNameResolver,
    IOptions<ElasticsearchOptions> options,
    ILogger<ElasticOrderSearchService> logger) : IOrderSearchIndexer, IOrderSearchReader
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ElasticsearchOptions _options = options.Value;
    private readonly SemaphoreSlim _indexInitializationLock = new(1, 1);
    private volatile bool _indexInitialized;

    public async Task IndexAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return;
        }

        var indexName = indexNameResolver.Resolve("orders");
        await EnsureIndexAsync(indexName, cancellationToken);

        var document = OrderSearchDocument.FromOrder(order);
        using var request = new HttpRequestMessage(HttpMethod.Put, $"/{indexName}/_doc/{document.OrderId}")
        {
            Content = CreateJsonContent(document)
        };

        using var response = await httpClientFactory.CreateClient("elasticsearch").SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Indexed order {OrderId} into Elasticsearch index {IndexName}.", order.Id, indexName);
    }

    public async Task<IReadOnlyCollection<OrderSearchResult>> SearchAsync(
        string? keyword,
        Guid? customerId,
        int size = 20,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return [];
        }

        var indexName = indexNameResolver.Resolve("orders");
        await EnsureIndexAsync(indexName, cancellationToken);

        var payload = BuildSearchPayload(keyword, customerId, size);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/{indexName}/_search")
        {
            Content = CreateJsonContent(payload)
        };

        using var response = await httpClientFactory.CreateClient("elasticsearch").SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        response.EnsureSuccessStatusCode();
        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);

        var hits = document.RootElement
            .GetProperty("hits")
            .GetProperty("hits")
            .EnumerateArray();

        var results = new List<OrderSearchResult>();
        foreach (var hit in hits)
        {
            if (!hit.TryGetProperty("_source", out var source))
            {
                continue;
            }

            var searchDocument = source.Deserialize<OrderSearchDocument>(JsonSerializerOptions);
            if (searchDocument is not null)
            {
                results.Add(searchDocument.ToSearchResult());
            }
        }

        logger.LogDebug(
            "Elasticsearch order search completed. Keyword={Keyword}, CustomerId={CustomerId}, Hits={Count}.",
            keyword,
            customerId,
            results.Count);

        return results;
    }

    private async Task EnsureIndexAsync(string indexName, CancellationToken cancellationToken)
    {
        if (_indexInitialized)
        {
            return;
        }

        await _indexInitializationLock.WaitAsync(cancellationToken);
        try
        {
            if (_indexInitialized)
            {
                return;
            }

            var client = httpClientFactory.CreateClient("elasticsearch");
            using var existsResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"/{indexName}"), cancellationToken);
            if (existsResponse.IsSuccessStatusCode)
            {
                _indexInitialized = true;
                return;
            }

            if (existsResponse.StatusCode != HttpStatusCode.NotFound)
            {
                existsResponse.EnsureSuccessStatusCode();
            }

            var indexDefinition = new
            {
                mappings = new
                {
                    properties = new Dictionary<string, object>
                    {
                        ["orderId"] = new { type = "keyword" },
                        ["customerId"] = new { type = "keyword" },
                        ["currency"] = new { type = "keyword" },
                        ["totalAmount"] = new { type = "double" },
                        ["itemCount"] = new { type = "integer" },
                        ["indexedAtUtc"] = new { type = "date" },
                        ["lineProductIds"] = new { type = "keyword" },
                        ["searchText"] = new { type = "text" },
                        ["lines"] = new
                        {
                            type = "nested",
                            properties = new Dictionary<string, object>
                            {
                                ["productId"] = new { type = "keyword" },
                                ["quantity"] = new { type = "integer" },
                                ["unitPrice"] = new { type = "double" }
                            }
                        }
                    }
                }
            };

            using var createResponse = await client.PutAsync($"/{indexName}", CreateJsonContent(indexDefinition), cancellationToken);
            if (createResponse.IsSuccessStatusCode)
            {
                _indexInitialized = true;
                return;
            }

            if (createResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var body = await createResponse.Content.ReadAsStringAsync(cancellationToken);
                if (body.Contains("resource_already_exists_exception", StringComparison.OrdinalIgnoreCase))
                {
                    _indexInitialized = true;
                    return;
                }
            }

            createResponse.EnsureSuccessStatusCode();
            _indexInitialized = true;
        }
        finally
        {
            _indexInitializationLock.Release();
        }
    }

    private static object BuildSearchPayload(string? keyword, Guid? customerId, int size)
    {
        var must = new List<object>();
        if (customerId.HasValue)
        {
            must.Add(new Dictionary<string, object>
            {
                ["term"] = new Dictionary<string, object>
                {
                    ["customerId"] = customerId.Value.ToString("D")
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            must.Add(new Dictionary<string, object>
            {
                ["bool"] = new Dictionary<string, object>
                {
                    ["should"] = new object[]
                    {
                        new Dictionary<string, object>
                        {
                            ["term"] = new Dictionary<string, object> { ["orderId"] = keyword }
                        },
                        new Dictionary<string, object>
                        {
                            ["term"] = new Dictionary<string, object> { ["customerId"] = keyword }
                        },
                        new Dictionary<string, object>
                        {
                            ["term"] = new Dictionary<string, object> { ["lineProductIds"] = keyword }
                        },
                        new Dictionary<string, object>
                        {
                            ["match"] = new Dictionary<string, object>
                            {
                                ["searchText"] = new Dictionary<string, object>
                                {
                                    ["query"] = keyword,
                                    ["operator"] = "and"
                                }
                            }
                        }
                    },
                    ["minimum_should_match"] = 1
                }
            });
        }

        var query = must.Count == 0
            ? new Dictionary<string, object> { ["match_all"] = new { } }
            : new Dictionary<string, object>
            {
                ["bool"] = new Dictionary<string, object>
                {
                    ["must"] = must
                }
            };

        return new
        {
            size = Math.Clamp(size, 1, 100),
            sort = new[]
            {
                new Dictionary<string, object>
                {
                    ["indexedAtUtc"] = new Dictionary<string, object> { ["order"] = "desc" }
                }
            },
            query
        };
    }

    private static StringContent CreateJsonContent(object payload)
    {
        return new StringContent(JsonSerializer.Serialize(payload, JsonSerializerOptions), Encoding.UTF8, "application/json");
    }
}
