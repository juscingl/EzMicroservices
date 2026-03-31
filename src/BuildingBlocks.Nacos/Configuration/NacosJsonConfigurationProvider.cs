using System.Text.Json;
using BuildingBlocks.Nacos.Options;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Nacos.Configuration;

public sealed class NacosJsonConfigurationProvider(NacosOptions options) : ConfigurationProvider
{
    public override void Load()
    {
        if (!options.Enabled || !options.LoadConfiguration || string.IsNullOrWhiteSpace(options.ConfigDataId))
        {
            return;
        }

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.ServerAddress),
            Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds)
        };

        var token = GetAccessTokenAsync(httpClient).GetAwaiter().GetResult();
        var requestUri = BuildConfigUri(token);
        var payload = httpClient.GetStringAsync(requestUri).GetAwaiter().GetResult();

        if (string.IsNullOrWhiteSpace(payload))
        {
            return;
        }

        using var document = JsonDocument.Parse(payload);
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        Flatten(document.RootElement, parentPath: null, data);
        Data = data;
    }

    private async Task<string?> GetAccessTokenAsync(HttpClient httpClient)
    {
        if (string.IsNullOrWhiteSpace(options.UserName) || string.IsNullOrWhiteSpace(options.Password))
        {
            return null;
        }

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = options.UserName,
            ["password"] = options.Password
        });

        using var response = await httpClient.PostAsync("/nacos/v1/auth/users/login", content);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.TryGetProperty("accessToken", out var accessToken)
            ? accessToken.GetString()
            : null;
    }

    private string BuildConfigUri(string? accessToken)
    {
        var query = new List<string>
        {
            $"dataId={Uri.EscapeDataString(options.ConfigDataId!)}",
            $"group={Uri.EscapeDataString(options.Group)}",
            $"tenant={Uri.EscapeDataString(options.NamespaceId)}"
        };

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            query.Add($"accessToken={Uri.EscapeDataString(accessToken)}");
        }

        return $"/nacos/v2/cs/config?{string.Join('&', query)}";
    }

    private static void Flatten(JsonElement element, string? parentPath, IDictionary<string, string?> data)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var childPath = string.IsNullOrWhiteSpace(parentPath)
                        ? property.Name
                        : $"{parentPath}:{property.Name}";
                    Flatten(property.Value, childPath, data);
                }
                break;
            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    Flatten(item, $"{parentPath}:{index}", data);
                    index++;
                }
                break;
            case JsonValueKind.Null:
                data[parentPath!] = null;
                break;
            default:
                data[parentPath!] = element.ToString();
                break;
        }
    }
}
