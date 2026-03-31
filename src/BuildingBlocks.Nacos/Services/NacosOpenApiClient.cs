using System.Text.Json;
using BuildingBlocks.Nacos.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Nacos.Services;

internal sealed class NacosOpenApiClient(
    HttpClient httpClient,
    IOptions<NacosOptions> options,
    ILogger<NacosOpenApiClient> logger) : INacosOpenApiClient
{
    private readonly NacosOptions _options = options.Value;
    private readonly SemaphoreSlim _tokenGate = new(1, 1);
    private string? _accessToken;
    private DateTime _accessTokenExpiresAtUtc = DateTime.MinValue;

    public async Task<string?> GetConfigAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ConfigDataId))
        {
            return null;
        }

        var token = await GetAccessTokenAsync(cancellationToken);
        using var response = await httpClient.GetAsync(BuildConfigUri(token), cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task RegisterInstanceAsync(string ip, int port, CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, string>
        {
            ["serviceName"] = _options.ServiceName ?? throw new InvalidOperationException("Nacos service name is required."),
            ["groupName"] = _options.Group,
            ["namespaceId"] = _options.NamespaceId,
            ["clusterName"] = _options.ClusterName,
            ["ip"] = ip,
            ["port"] = port.ToString(),
            ["ephemeral"] = bool.TrueString.ToLowerInvariant()
        };

        if (_options.Metadata.Count > 0)
        {
            payload["metadata"] = JsonSerializer.Serialize(_options.Metadata);
        }

        await SendInstanceRequestAsync(HttpMethod.Post, payload, cancellationToken);
    }

    public async Task DeregisterInstanceAsync(string ip, int port, CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, string>
        {
            ["serviceName"] = _options.ServiceName ?? throw new InvalidOperationException("Nacos service name is required."),
            ["groupName"] = _options.Group,
            ["namespaceId"] = _options.NamespaceId,
            ["clusterName"] = _options.ClusterName,
            ["ip"] = ip,
            ["port"] = port.ToString(),
            ["ephemeral"] = bool.TrueString.ToLowerInvariant()
        };

        await SendInstanceRequestAsync(HttpMethod.Delete, payload, cancellationToken);
    }

    private async Task SendInstanceRequestAsync(
        HttpMethod method,
        IReadOnlyDictionary<string, string> payload,
        CancellationToken cancellationToken)
    {
        var token = await GetAccessTokenAsync(cancellationToken);
        var query = new List<string>();
        foreach (var pair in payload)
        {
            query.Add($"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}");
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            query.Add($"accessToken={Uri.EscapeDataString(token)}");
        }

        using var request = new HttpRequestMessage(method, $"/nacos/v2/ns/instance?{string.Join('&', query)}");
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "Nacos instance request failed. Method={Method}, StatusCode={StatusCode}, Response={Response}",
                method,
                response.StatusCode,
                body);
        }

        response.EnsureSuccessStatusCode();
    }

    private async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.UserName) || string.IsNullOrWhiteSpace(_options.Password))
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(_accessToken) && _accessTokenExpiresAtUtc > DateTime.UtcNow)
        {
            return _accessToken;
        }

        await _tokenGate.WaitAsync(cancellationToken);
        try
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && _accessTokenExpiresAtUtc > DateTime.UtcNow)
            {
                return _accessToken;
            }

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = _options.UserName!,
                ["password"] = _options.Password!
            });

            using var response = await httpClient.PostAsync("/nacos/v1/auth/users/login", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            _accessToken = document.RootElement.TryGetProperty("accessToken", out var accessToken)
                ? accessToken.GetString()
                : null;

            var tokenTtlSeconds = document.RootElement.TryGetProperty("tokenTtl", out var tokenTtl)
                ? tokenTtl.GetInt32()
                : 18000;

            _accessTokenExpiresAtUtc = DateTime.UtcNow.AddSeconds(Math.Max(tokenTtlSeconds - 60, 60));
            return _accessToken;
        }
        finally
        {
            _tokenGate.Release();
        }
    }

    private string BuildConfigUri(string? accessToken)
    {
        var query = new List<string>
        {
            $"dataId={Uri.EscapeDataString(_options.ConfigDataId!)}",
            $"group={Uri.EscapeDataString(_options.Group)}",
            $"tenant={Uri.EscapeDataString(_options.NamespaceId)}"
        };

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            query.Add($"accessToken={Uri.EscapeDataString(accessToken)}");
        }

        return $"/nacos/v2/cs/config?{string.Join('&', query)}";
    }
}
